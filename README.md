# Edi.WordFilter

Basic word filter used in my blog system to filter dirty words (e.g. insulting languages, impertinent words)

[![NuGet][main-nuget-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/Edi.WordFilter/
[main-nuget-badge]: https://img.shields.io/nuget/v/Edi.WordFilter.svg?style=flat-square&label=nuget

## Install

```
dotnet add package Edi.WordFilter
```

## Usage

Prepare a text file with banned words, for example splitted by `|`. Like this:

```
fuck|shit|ass
```

Use it like this

```
var wordFilterDataFilePath = $"{AppDomain.CurrentDomain.GetData(Constants.DataDirectory)}\\BannedWords.txt";
var filter = new TrieTreeWordFilter(wordFilterDataFilePath);

var output = filter.FilterContent("Go fuck yourself and eat some shit!");
// output: Go **** yourself and eat some ****!
```

### Design Details

#### TrieTreeWordFilter

The `FilterContent()` method in the `TrieTreeWordFilter` class is used to filter sensitive words from a given string content. It uses a Trie data structure to efficiently find and replace sensitive words with asterisks (*).

This method is efficient for filtering sensitive words, especially when there is a large set of words to filter. However, it assumes that the Trie tree has been properly initialized with all the sensitive words.

A Trie, also known as a prefix tree, is a tree-like data structure that is used to store a collection of strings. Each node of the Trie represents a character of a string and the root of the Trie represents an empty string or the start of a string. The strings are stored in a way that all the descendants of a node have a common prefix of the string associated with that node.
Here's a simple example of how a Trie might look when storing the words "car", "cat", and "dog":

```
root
├── c
│   ├── a
│   │   ├── r
│   │   └── t
└── d
    └── o
        └── g
```

In this example, each path from the root to a node represents a string. For instance, the path from the root to the node 'r' represents the string "car".

Tries are particularly useful for operations that involve prefix matching, such as autocomplete features in text editors or web browsers, as they allow for efficient retrieval of all keys with a given prefix. They are also used in word filtering, as in the FilterContent() method you asked about earlier.

However, Tries can be memory-intensive, as each node may need to store pointers to many children. There are variations of the Trie data structure, such as the compressed Trie (also known as a Radix tree or Patricia tree), which help to mitigate this issue by merging nodes with a single child.

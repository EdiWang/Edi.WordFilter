using System;
using System.Collections.Generic;

namespace Edi.WordFilter;

public class TrieNode
{
    public Dictionary<char, TrieNode> Children = new();
    public bool IsEndOfWord;
}

public class TrieTreeWordFilter : IMaskWordFilter
{
    private readonly TrieNode _root = new();

    public TrieTreeWordFilter(IWordSource wordSource)
    {
        var banWords = wordSource.GetWordsArray();
        foreach (var s in banWords) AddWord(s);
    }

    public void AddWord(string word)
    {
        var current = _root;
        foreach (var ch in word)
        {
            if (!current.Children.TryGetValue(ch, out var node))
            {
                node = new();
                current.Children.Add(ch, node);
            }
            current = node;
        }
        current.IsEndOfWord = true;
    }

    public bool ContainsAnyWord(string content)
    {
        throw new NotImplementedException();
    }

    public string FilterContent(string content)
    {
        var result = new char[content.Length];
        var current = _root;
        int slowIndex = 0, fastIndex = 0;

        while (fastIndex < content.Length)
        {
            var ch = content[fastIndex];
            if (current.Children.TryGetValue(ch, out var node))
            {
                // Found a starting character of a word
                if (node.IsEndOfWord)
                {
                    // Found a complete sensitive word, replace it with '*'
                    for (var i = slowIndex; i <= fastIndex; i++)
                    {
                        result[i] = '*';
                    }
                    // Reset trie traversal to the root
                    current = _root;
                    slowIndex = fastIndex + 1;
                }
                else
                {
                    // Continue to the next character
                    current = node;
                }
                fastIndex++;
            }
            else
            {
                // Current path does not lead to a sensitive word
                result[slowIndex] = content[slowIndex];
                slowIndex++;
                // If not starting from the root (inside a potential word)
                if (current != _root)
                {
                    fastIndex = slowIndex;
                    current = _root;
                }
                else
                {
                    fastIndex++;
                }
            }
        }

        // Copy the remaining characters
        while (slowIndex < content.Length)
        {
            result[slowIndex] = content[slowIndex];
            slowIndex++;
        }

        return new(result);
    }
}
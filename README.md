# Edi.WordFilter

Module used in my blog system to filter dirty words (e.g. insulting languages, impertinent words)

It uses HashTable to filter content very quickly.

![.NET Build Linux](https://github.com/EdiWang/Edi.WordFilter/workflows/.NET%20Build%20Linux/badge.svg)

[![NuGet][main-nuget-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/Edi.WordFilter/
[main-nuget-badge]: https://img.shields.io/nuget/v/Edi.WordFilter.svg?style=flat-square&label=nuget

## Usage

**1.Prepare a text file with banned words, for example splitted by "|". Like this:**
```
fuck|shit|ass
```
Because I still want to live, I can't give you the entire keywords list, sorry for that.

**2.Install from NuGet:**
```
Install-Package Edi.WordFilter
```

**3.Use it like this:**
```
var wordFilterDataFilePath = $"{AppDomain.CurrentDomain.GetData(Constants.DataDirectory)}\\BannedWords.txt";
var maskWordFilter = new MaskWordFilter(wordFilterDataFilePath);
username = maskWordFilter.FilterContent(username);
commentContent = maskWordFilter.FilterContent(commentContent);
```

**4.The disharmony words will be replaced by "*"**

### Design Details

Split disharmony word into HashTable, Key points to the first character, Value points to the next character where Value itself is the Key of the next HashTable. When filtering content, begin search with the first HashTable, if matching double side, then it is a disharmony word.

For example, given disharmony word **"FUCK,FS,ABC"**, the following structure is created:

![image](https://raw.githubusercontent.com/EdiWang/Edi.WordFilter/master/img/doc-hashtable-structure.png)

Each blue box represents a HashTable, each character of the disharmony word is stored as the Key and pointing to the next HashTable.

For example, if user input **"FUCK FAKE"**, the flow is:

"F" can be found in the first level of HashTable(H0), "U" can be found in the HashTable(H1) where H1's value is reffered to, like this, C and K can be found in H2 and H3, so "FUCK" is a disharmony word.

For the word "FAKE", although "F" can be found in HashTable(H0), but H0 does not have a value pointing to "A", and "A" also don't have "K" sits in it's value, and "K" also does not exists in the first level of HashTable, so "FAKE" is not a disharmony word.

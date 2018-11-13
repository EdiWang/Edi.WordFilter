# Edi.WordFilter

Module used in my blog system to filter disharmony words in order to live in China.

It uses hashtable to filter content very quickly.

## Usage
1. Prepare a text file with banned words, for example splitted by "|". Like this:
```
翻墙|防火长城|GFW|fuck
```
Because I still want to live, I can't give you the entire keywords list, sorry for that.

2. Install from NuGet: Install-Package Edi.WordFilter

3. Use it like this:
```
var wordFilterDataFilePath = $"{AppDomain.CurrentDomain.GetData(Constants.DataDirectory)}\\BannedWords.txt";
var maskWordFilter = new MaskWordFilter(wordFilterDataFilePath);
username = maskWordFilter.FilterContent(username);
commentContent = maskWordFilter.FilterContent(commentContent);
```

4. The disharmony words will be replaced by "*"

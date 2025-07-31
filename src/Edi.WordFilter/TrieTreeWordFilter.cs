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
        foreach (var s in banWords) AddWord(s.ToLower());
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
        for (int i = 0; i < content.Length; i++)
        {
            var current = _root;
            int j = i;

            while (j < content.Length)
            {
                var ch = char.ToLower(content[j]);
                if (current.Children.TryGetValue(ch, out var node))
                {
                    current = node;
                    if (current.IsEndOfWord)
                    {
                        return true;
                    }
                    j++;
                }
                else
                {
                    break;
                }
            }
        }

        return false;
    }

    public string FilterContent(string content)
    {
        var result = content.ToCharArray();

        for (int i = 0; i < content.Length; i++)
        {
            var current = _root;
            int j = i;
            int lastEndOfWordIndex = -1;

            // Find the longest matching word starting at position i
            while (j < content.Length)
            {
                var ch = char.ToLower(content[j]);
                if (current.Children.TryGetValue(ch, out var node))
                {
                    current = node;
                    if (current.IsEndOfWord)
                    {
                        lastEndOfWordIndex = j;
                    }
                    j++;
                }
                else
                {
                    break;
                }
            }

            // If we found a complete word, mask it with the longest match
            if (lastEndOfWordIndex != -1)
            {
                for (int k = i; k <= lastEndOfWordIndex; k++)
                {
                    result[k] = '*';
                }
                // Skip to the end of the masked word
                i = lastEndOfWordIndex;
            }
        }

        return new string(result);
    }
}
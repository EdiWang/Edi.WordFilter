using System;
using System.Collections.Generic;
using System.Text;

namespace Edi.WordFilter;

public sealed class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; set; }
    public bool IsEndOfWord { get; set; }

    public void AddChild(char ch, TrieNode node)
    {
        Children ??= [];
        Children[ch] = node;
    }

    public bool TryGetChild(char ch, out TrieNode node)
    {
        node = null;
        return Children?.TryGetValue(ch, out node) == true;
    }
}

public sealed class TrieTreeWordFilter : IMaskWordFilter
{
    private readonly TrieNode _root = new();

    public TrieTreeWordFilter(IWordSource wordSource)
    {
        var banWords = wordSource.GetWordsArray();
        foreach (var word in banWords)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                AddWord(word.ToLowerInvariant());
            }
        }
    }

    public void AddWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return;

        var current = _root;
        foreach (var ch in word)
        {
            if (!current.TryGetChild(ch, out var node))
            {
                node = new TrieNode();
                current.AddChild(ch, node);
            }
            current = node!;
        }
        current.IsEndOfWord = true;
    }

    public bool ContainsAnyWord(string content)
    {
        if (string.IsNullOrEmpty(content)) return false;

        var contentSpan = content.AsSpan();

        for (int i = 0; i < contentSpan.Length; i++)
        {
            var current = _root;
            int j = i;

            while (j < contentSpan.Length)
            {
                var ch = char.ToLowerInvariant(contentSpan[j]);
                if (current.TryGetChild(ch, out var node))
                {
                    current = node!;
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
        if (string.IsNullOrEmpty(content)) return content;

        var result = new StringBuilder(content);
        var contentSpan = content.AsSpan();

        for (int i = 0; i < contentSpan.Length; i++)
        {
            var current = _root;
            int j = i;
            int lastEndOfWordIndex = -1;

            // Find the longest matching word starting at position i
            while (j < contentSpan.Length)
            {
                var ch = char.ToLowerInvariant(contentSpan[j]);
                if (current.TryGetChild(ch, out var node))
                {
                    current = node!;
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

        return result.ToString();
    }
}
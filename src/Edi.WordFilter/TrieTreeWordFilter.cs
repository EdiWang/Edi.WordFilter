using System;
using System.Collections.Generic;
using System.Text;
using System.Buffers;

namespace Edi.WordFilter;

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

        var contentSpan = content.AsSpan();
        Span<char> result = stackalloc char[content.Length <= 1024 ? content.Length : 0];
        char[] rentedArray = null;
        
        if (content.Length > 1024)
        {
            rentedArray = ArrayPool<char>.Shared.Rent(content.Length);
            result = rentedArray.AsSpan(0, content.Length);
        }

        try
        {
            contentSpan.CopyTo(result);
            
            for (int i = 0; i < result.Length; i++)
            {
                var current = _root;
                int j = i;
                int lastEndOfWordIndex = -1;

                while (j < result.Length)
                {
                    var ch = char.ToLowerInvariant(result[j]);
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

                if (lastEndOfWordIndex != -1)
                {
                    result.Slice(i, lastEndOfWordIndex - i + 1).Fill('*');
                    i = lastEndOfWordIndex;
                }
            }

            return new string(result);
        }
        finally
        {
            if (rentedArray != null)
            {
                ArrayPool<char>.Shared.Return(rentedArray);
            }
        }
    }
}
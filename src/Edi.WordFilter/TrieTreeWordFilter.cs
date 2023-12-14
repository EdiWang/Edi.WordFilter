using System;
using System.Collections.Generic;

namespace Edi.WordFilter;

public class TrieNode
{
    public Dictionary<char, TrieNode> Children = new();
    public bool IsEndOfWord = false;
}

public class TrieTreeWordFilter : IMaskWordFilter
{
    private TrieNode root = new TrieNode();

    public TrieTreeWordFilter(IWordSource wordSource)
    {
        var banWords = wordSource.GetWordsArray();
        foreach (var s in banWords) AddWord(s);
    }

    public void AddWord(string word)
    {
        TrieNode current = root;
        foreach (char ch in word)
        {
            if (!current.Children.TryGetValue(ch, out TrieNode node))
            {
                node = new TrieNode();
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
        char[] result = new char[content.Length];
        TrieNode current = root;
        int slowIndex = 0, fastIndex = 0;

        while (fastIndex < content.Length)
        {
            char ch = content[fastIndex];
            if (current.Children.TryGetValue(ch, out TrieNode node))
            {
                // Found a starting character of a word
                if (node.IsEndOfWord)
                {
                    // Found a complete sensitive word, replace it with '*'
                    for (int i = slowIndex; i <= fastIndex; i++)
                    {
                        result[i] = '*';
                    }
                    // Reset trie traversal to the root
                    current = root;
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
                if (current != root)
                {
                    fastIndex = slowIndex;
                    current = root;
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

        return new string(result);
    }
}
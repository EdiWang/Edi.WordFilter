using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        throw new NotImplementedException();
    }
}
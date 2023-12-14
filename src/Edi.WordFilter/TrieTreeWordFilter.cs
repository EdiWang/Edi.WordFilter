﻿using System;
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
    public bool ContainsAnyWord(string content)
    {
        throw new NotImplementedException();
    }

    public string FilterContent(string content)
    {
        throw new NotImplementedException();
    }
}
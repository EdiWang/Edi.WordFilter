using System.Collections.Generic;

public sealed class TrieNode
{
    // Use array for ASCII characters (0-127) and dictionary for others
    private TrieNode[] _asciiChildren;
    private Dictionary<char, TrieNode> _nonAsciiChildren;
    public bool IsEndOfWord { get; set; }

    public void AddChild(char ch, TrieNode node)
    {
        if (ch < 128)
        {
            _asciiChildren ??= new TrieNode[128];
            _asciiChildren[ch] = node;
        }
        else
        {
            _nonAsciiChildren ??= [];
            _nonAsciiChildren[ch] = node;
        }
    }

    public bool TryGetChild(char ch, out TrieNode node)
    {
        if (ch < 128)
        {
            node = _asciiChildren?[ch];
            return node != null;
        }
        
        node = null;
        return _nonAsciiChildren?.TryGetValue(ch, out node) == true;
    }
}
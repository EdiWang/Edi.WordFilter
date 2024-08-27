namespace Edi.WordFilter;

public class StringWordSource(string words, char splitChar = '|') : IWordSource
{
    public char SplitChar { get; } = splitChar;

    public string Words { get; } = words;

    public string[] GetWordsArray()
    {
        return Words.Split(SplitChar);
    }
}
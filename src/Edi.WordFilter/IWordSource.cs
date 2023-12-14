namespace Edi.WordFilter
{
    public interface IWordSource
    {
        char SplitChar { get; }

        string[] GetWordsArray();
    }
}

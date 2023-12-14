namespace Edi.WordFilter;

public interface IMaskWordFilter
{
    bool ContainsAnyWord(string content);
    string FilterContent(string content);
}
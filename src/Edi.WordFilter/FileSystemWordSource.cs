using System.IO;
using System.Text;

namespace Edi.WordFilter;

public class FileSystemWordSource(string dataFilePath, char splitChar = '|') : IWordSource
{
    public string DataFilePath { get; } = dataFilePath;

    public char SplitChar { get; set; } = splitChar;

    public string[] GetWordsArray()
    {
        var fileContent = GetBanWordFromDataFile();
        var words = fileContent.Split(SplitChar);
        return words;
    }

    private string GetBanWordFromDataFile()
    {
        using var reader = new StreamReader(DataFilePath, Encoding.UTF8);
        var content = reader.ReadLine();
        return content;
    }
}
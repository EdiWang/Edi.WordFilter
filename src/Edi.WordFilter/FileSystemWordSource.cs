using System.IO;
using System.Text;

namespace Edi.WordFilter
{
    public class FileSystemWordSource : IWordSource
    {
        public string DataFilePath { get; }

        public char SplitChar { get; set; }

        public FileSystemWordSource(string dataFilePath, char splitChar = '|')
        {
            DataFilePath = dataFilePath;
            SplitChar = splitChar;
        }

        public string[] GetWordsArray()
        {
            var fileContent = GetBanWordFromDataFile();
            var words = fileContent.Split(SplitChar);
            return words;
        }

        private string GetBanWordFromDataFile()
        {
            using (var reader = new StreamReader(DataFilePath, Encoding.UTF8))
            {
                var content = reader.ReadLine();
                return content;
            }
        }
    }
}

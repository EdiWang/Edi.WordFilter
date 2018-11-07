using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Edi.WordFilter
{
    public class MaskWordFilter
    {
        public string DataFilePath { get; }

        public char SplitChar { get; }

        private readonly Hashtable _filterWords = new Hashtable();

        private readonly object _objLock = new object();

        private string BanWords { get; }

        public MaskWordFilter(string dataFilePath, char splitChar = '|')
        {
            SplitChar = splitChar;
            DataFilePath = dataFilePath;
            BanWords = GetBanWordFromDataFile();
            var banWords = BanWords.Split(SplitChar);
            foreach (var s in banWords) AddWordToHashtable(s);
        }

        public string FilterContent(string content)
        {
            lock (_filterWords)
            {
                var result = new StringBuilder();
                var filterChar = true;

                for (var i = 0; i < content.Length; i++)
                {
                    var c = content[i];
                    switch (c)
                    {
                        case '<':
                            {
                                filterChar = false;
                                break;
                            }
                        case '>':
                            {
                                filterChar = true;
                                break;
                            }
                        default:
                            {
                                if (filterChar)
                                {
                                    var fi = Match(content, i, out var temp);
                                    if (fi != -1)
                                    {
                                        i = fi;
                                        result.Append(temp);
                                        continue;
                                    }
                                }

                                break;
                            }
                    }

                    result.Append(c);
                }

                return result.ToString();
            }
        }

        public void SaveDataFile()
        {
            SaveDataFile(BanWords);
        }

        #region Private Methods

        private void AddWordToHashtable(string word)
        {
            var h = _filterWords;
            foreach (var c in word.ToUpper())
            {
                if (h != null && !h.ContainsKey(c)) h[c] = new Hashtable();
                h = h?[c] as Hashtable;
            }

            if (h != null) h[0] = new Hashtable();
        }

        private int Match(string content, int index, out StringBuilder alt)
        {
            content = content.ToUpper();
            alt = new StringBuilder();
            var filterChar = true;

            var h = _filterWords;
            var i = index;
            for (; i < content.Length; i++)
            {
                var c = content[i];
                switch (c)
                {
                    case '<':
                        filterChar = false;
                        break;
                    case '>':
                    case '*':
                    case '，':
                    case ',':
                        filterChar = true;
                        break;
                    case ' ':
                        break;
                    default:
                        {
                            if (filterChar)
                            {
                                if (h != null && h.ContainsKey(c))
                                {
                                    h = h[c] as Hashtable;
                                    c = '*';
                                }
                                else
                                {
                                    if (!h.ContainsKey(0)) return -1;
                                }
                            }

                            break;
                        }
                }

                alt.Append(c);
                if (h != null && h.ContainsKey(0)) return i;
            }

            return h != null && h.ContainsKey(0) ? i : -1;
        }

        private string GetBanWordFromDataFile()
        {
            using (var reader = new StreamReader(DataFilePath, Encoding.UTF8))
            {
                var content = reader.ReadLine();
                return content;
            }
        }

        private void SaveDataFile(string banWords)
        {
            lock (_objLock)
            {
                using (var writer = File.CreateText(DataFilePath))
                {
                    writer.WriteLine(FormatKeyWord(banWords));
                    writer.Flush();
                }
            }
        }

        private string FormatKeyWord(string banWordsList)
        {
            banWordsList = banWordsList.Replace("'", string.Empty).Replace(" ", string.Empty);
            banWordsList = banWordsList.Trim(SplitChar);
            return banWordsList;
        }

        #endregion
    }
}

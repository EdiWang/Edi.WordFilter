using System;
using System.Collections.Generic;
using System.Text;

namespace Edi.WordFilter
{
    public class StringWordSource : IWordSource
    {
        public char SplitChar { get; }

        public string Words { get; }

        public StringWordSource(string words, char splitChar = '|')
        {
            Words = words;
            SplitChar = splitChar;
        }

        public string[] GetWordsArray()
        {
            return Words.Split(SplitChar);
        }
    }
}

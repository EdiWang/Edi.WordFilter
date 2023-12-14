using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Edi.WordFilter
{
    public interface IMaskWordFilter
    {
        bool ContainsAnyWord(string content);
        string FilterContent(string content);
    }

    public class HashTableWordFilter : IMaskWordFilter
    {
        private readonly IDictionary<char, IDictionary> _filterWords = new Dictionary<char, IDictionary>();

        public HashTableWordFilter(IWordSource wordSource)
        {
            var banWords = wordSource.GetWordsArray();
            foreach (var s in banWords) AddWordToHashtable(s);
        }

        public bool ContainsAnyWord(string content)
        {
            lock (_filterWords)
            {
                var filterChar = true;

                for (var i = 0; i < content.Length; i++)
                {
                    var c = content[i];
                    switch (c)
                    {
                        case '<':
                            filterChar = false;
                            break;
                        case '>':
                            filterChar = true;
                            break;
                        default:
                            if (filterChar)
                            {
                                var fi = Match(content, i, out var temp);
                                if (fi != -1) return true;
                            }
                            break;
                    }
                }

                return false;
            }
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
                            filterChar = false;
                            break;
                        case '>':
                            filterChar = true;
                            break;
                        default:
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

                    result.Append(c);
                }

                return result.ToString();
            }
        }

        #region Private Methods

        private void AddWordToHashtable(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return;

            var h = _filterWords;
            foreach (var c in word.ToUpper())
            {
                if (h != null && !h.ContainsKey(c)) h[c] = new Dictionary<char, IDictionary>();
                h = h?[c] as IDictionary<char, IDictionary>;
            }

            if (h != null) h['0'] = new Dictionary<char, IDictionary>();
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
                        if (filterChar)
                        {
                            if (h != null && h.ContainsKey(c))
                            {
                                h = h[c] as IDictionary<char, IDictionary>;
                                c = '*';
                            }
                            else
                            {
                                if (h != null && !h.ContainsKey('0')) return -1;
                            }
                        }

                        break;
                }

                alt.Append(c);
                if (h != null && h.ContainsKey('0')) return i;
            }

            return h != null && h.ContainsKey('0') ? i : -1;
        }

        #endregion
    }
}

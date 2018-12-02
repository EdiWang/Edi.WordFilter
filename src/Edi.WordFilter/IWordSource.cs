using System;
using System.Collections.Generic;
using System.Text;

namespace Edi.WordFilter
{
    public interface IWordSource
    {
        char SplitChar { get; }

        string[] GetWordsArray();
    }
}

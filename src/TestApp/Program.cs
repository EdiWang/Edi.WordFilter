using System;
using Edi.WordFilter;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var disharmonyStr = "Go fuck yourself and eat some shit!";
            var wf = new MaskWordFilter(new StringWordSource("fuck|shit"));
            var harmonyStr = wf.FilterContent(disharmonyStr);

            Console.WriteLine(harmonyStr);
        }
    }
}

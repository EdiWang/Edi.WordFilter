using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edi.WordFilter.Tests;

[TestFixture]
public class TrieTreeWordFilterTests
{
    public IMaskWordFilter MaskWordFilter { get; set; }

    [Test]
    public void HarmonizeWords()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.That("Go **** yourself and eat some ****!", Is.EqualTo(harmonyStr));
    }
}
using NUnit.Framework;

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

    [Test]
    public void HarmonizeWords_MessedUpSource()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit|"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.That("Go **** yourself and eat some ****!", Is.EqualTo(harmonyStr));
    }
}
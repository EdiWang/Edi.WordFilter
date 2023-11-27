using NUnit.Framework;

namespace Edi.WordFilter.Tests;

[TestFixture]
public class Tests
{
    public IMaskWordFilter MaskWordFilter { get; set; }

    [Test]
    public void HarmonizeWords()
    {
        MaskWordFilter = new MaskWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.That("Go **** yourself and eat some ****!", Is.EqualTo(harmonyStr));
    }

    [Test]
    public void HarmonizeWords_MessedUpSource()
    {
        MaskWordFilter = new MaskWordFilter(new StringWordSource("fuck|shit|"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.That("Go **** yourself and eat some ****!", Is.EqualTo(harmonyStr));
    }

    [Test]
    public void ContainsAnyWord_Yes()
    {
        MaskWordFilter = new MaskWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var b = MaskWordFilter.ContainsAnyWord(disharmonyStr);
        Assert.That(b, Is.True);
    }

    [Test]
    public void ContainsAnyWord_No()
    {
        MaskWordFilter = new MaskWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go frack yourself and eat some shirt!";
        var b = MaskWordFilter.ContainsAnyWord(disharmonyStr);
        Assert.That(b, Is.False);
    }
}
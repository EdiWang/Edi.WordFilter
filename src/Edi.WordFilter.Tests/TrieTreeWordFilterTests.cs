using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edi.WordFilter.Tests;

[TestClass]
public class TrieTreeWordFilterTests
{
    public IMaskWordFilter MaskWordFilter { get; set; }

    [TestMethod]
    public void HarmonizeWords()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.AreEqual("Go **** yourself and eat some ****!", harmonyStr);
    }

    [TestMethod]
    public void HarmonizeWordsCaseInsensitive()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go FuCk yourself and eat some shiT!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.AreEqual("Go **** yourself and eat some ****!", harmonyStr);
    }

    [TestMethod]
    public void HarmonizeWords_MessedUpSource()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit|"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.AreEqual("Go **** yourself and eat some ****!", harmonyStr);
    }

    [TestMethod]
    public void ContainsAnyWord_Yes()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var b = MaskWordFilter.ContainsAnyWord(disharmonyStr);
        Assert.IsTrue(b);
    }

    [TestMethod]
    public void ContainsAnyWord_No()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go frack yourself and eat some shirt!";
        var b = MaskWordFilter.ContainsAnyWord(disharmonyStr);
        Assert.IsFalse(b);
    }
}
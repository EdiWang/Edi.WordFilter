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

    // New comprehensive tests for all methods

    #region Constructor Tests

    [TestMethod]
    public void Constructor_EmptyWordSource_CreatesEmptyFilter()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource(""));

        var result = MaskWordFilter.FilterContent("Any text here");
        Assert.AreEqual("Any text here", result);
        Assert.IsFalse(MaskWordFilter.ContainsAnyWord("Any text here"));
    }

    [TestMethod]
    public void Constructor_SingleWord_CreatesValidFilter()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This is bad"));
        Assert.AreEqual("This is ***", MaskWordFilter.FilterContent("This is bad"));
    }

    [TestMethod]
    public void Constructor_MultipleWords_CreatesValidFilter()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil|wrong"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This is bad and evil"));
        Assert.AreEqual("This is *** and ****", MaskWordFilter.FilterContent("This is bad and evil"));
    }

    [TestMethod]
    public void Constructor_WordsWithDifferentCases_ConvertsToLowerCase()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("BAD|Evil|WrOnG"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This is bad and evil and wrong"));
        Assert.AreEqual("This is *** and **** and *****", MaskWordFilter.FilterContent("This is bad and evil and wrong"));
    }

    #endregion

    #region AddWord Tests

    [TestMethod]
    public void AddWord_SingleCharacter_AddsCorrectly()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        addWordMethod.Invoke(filter, new object[] { "a" });

        Assert.IsTrue(filter.ContainsAnyWord("a"));
        Assert.AreEqual("*", filter.FilterContent("a"));
    }

    [TestMethod]
    public void AddWord_MultipleCharacters_AddsCorrectly()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        addWordMethod.Invoke(filter, new object[] { "test" });

        Assert.IsTrue(filter.ContainsAnyWord("This is a test"));
        Assert.AreEqual("This is a ****", filter.FilterContent("This is a test"));
    }

    [TestMethod]
    public void AddWord_EmptyString_DoesNotCrash()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        // Should not throw exception
        addWordMethod.Invoke(filter, new object[] { "" });

        Assert.IsFalse(filter.ContainsAnyWord("Any text"));
        Assert.AreEqual("Any text", filter.FilterContent("Any text"));
    }

    [TestMethod]
    public void AddWord_DuplicateWord_HandlesCorrectly()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        addWordMethod.Invoke(filter, new object[] { "test" });
        addWordMethod.Invoke(filter, new object[] { "test" }); // Add same word again

        Assert.IsTrue(filter.ContainsAnyWord("This is a test"));
        Assert.AreEqual("This is a ****", filter.FilterContent("This is a test"));
    }

    #endregion

    #region ContainsAnyWord Tests

    [TestMethod]
    public void ContainsAnyWord_EmptyContent_ReturnsFalse()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.IsFalse(MaskWordFilter.ContainsAnyWord(""));
    }

    [TestMethod]
    public void ContainsAnyWord_ContentWithoutBadWords_ReturnsFalse()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.IsFalse(MaskWordFilter.ContainsAnyWord("This is a good text"));
    }

    [TestMethod]
    public void ContainsAnyWord_ContentWithBadWordAtStart_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("bad text here"));
    }

    [TestMethod]
    public void ContainsAnyWord_ContentWithBadWordInMiddle_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This bad text"));
    }

    [TestMethod]
    public void ContainsAnyWord_ContentWithMultipleBadWords_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This bad and evil text"));
    }

    [TestMethod]
    public void ContainsAnyWord_CaseInsensitive_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This BAD text"));
        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This Evil text"));
        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This BaD text"));
    }

    [TestMethod]
    public void ContainsAnyWord_SingleCharacterWord_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("a|x"));

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("a"));
        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("x"));
        Assert.IsTrue(MaskWordFilter.ContainsAnyWord("This is a test"));
    }

    #endregion

    #region FilterContent Tests

    [TestMethod]
    public void FilterContent_EmptyContent_ReturnsEmpty()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.AreEqual("", MaskWordFilter.FilterContent(""));
    }

    [TestMethod]
    public void FilterContent_ContentWithoutBadWords_ReturnsOriginal()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        var content = "This is a good text";
        Assert.AreEqual(content, MaskWordFilter.FilterContent(content));
    }

    [TestMethod]
    public void FilterContent_ContentWithSingleBadWord_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("This *** text", MaskWordFilter.FilterContent("This bad text"));
    }

    [TestMethod]
    public void FilterContent_ContentWithMultipleBadWords_FiltersAll()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil|wrong"));

        Assert.AreEqual("This *** and **** and ***** text",
            MaskWordFilter.FilterContent("This bad and evil and wrong text"));
    }

    [TestMethod]
    public void FilterContent_BadWordAtStart_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("*** text here", MaskWordFilter.FilterContent("bad text here"));
    }

    [TestMethod]
    public void FilterContent_BadWordAtEnd_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("This text is ***", MaskWordFilter.FilterContent("This text is bad"));
    }

    [TestMethod]
    public void FilterContent_ConsecutiveBadWords_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.AreEqual("******* text", MaskWordFilter.FilterContent("badevil text"));
    }

    [TestMethod]
    public void FilterContent_CaseInsensitive_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("*** *** ***", MaskWordFilter.FilterContent("BAD Bad bAd"));
    }

    [TestMethod]
    public void FilterContent_SingleCharacterWord_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("a|x"));

        Assert.AreEqual("* test * here", MaskWordFilter.FilterContent("a test x here"));
    }

    [TestMethod]
    public void FilterContent_OverlappingWords_FiltersLongestMatch()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("test|testing"));

        Assert.AreEqual("*******", MaskWordFilter.FilterContent("testing"));
        Assert.AreEqual("****", MaskWordFilter.FilterContent("test"));
    }

    [TestMethod]
    public void FilterContent_SpecialCharacters_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("This is *** stuff!", MaskWordFilter.FilterContent("This is bad stuff!"));
        Assert.AreEqual("*** & good", MaskWordFilter.FilterContent("bad & good"));
    }

    [TestMethod]
    public void FilterContent_NumbersAndLetters_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad123|test"));

        Assert.AreEqual("****** and ****", MaskWordFilter.FilterContent("bad123 and test"));
    }

    [TestMethod]
    public void FilterContent_OnlyBadWord_ReturnsOnlyAsterisks()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("***", MaskWordFilter.FilterContent("bad"));
    }

    [TestMethod]
    public void FilterContent_RepeatedBadWords_FiltersAll()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("*** *** ***", MaskWordFilter.FilterContent("bad bad bad"));
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void FilterContent_VeryLongContent_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        var longContent = new string('a', 1000) + " bad " + new string('b', 1000);
        var expected = new string('a', 1000) + " *** " + new string('b', 1000);

        Assert.AreEqual(expected, MaskWordFilter.FilterContent(longContent));
    }

    [TestMethod]
    public void ContainsAnyWord_VeryLongContent_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        var longContent = new string('a', 1000) + " bad " + new string('b', 1000);

        Assert.IsTrue(MaskWordFilter.ContainsAnyWord(longContent));
    }

    [TestMethod]
    public void FilterContent_WhitespaceOnly_ReturnsOriginal()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.AreEqual("   ", MaskWordFilter.FilterContent("   "));
        Assert.AreEqual("\t\n\r", MaskWordFilter.FilterContent("\t\n\r"));
    }

    #endregion
}
using Xunit;

namespace Edi.WordFilter.Tests;

public class TrieTreeWordFilterTests
{
    public IMaskWordFilter MaskWordFilter { get; set; }

    [Fact]
    public void HarmonizeWords()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.Equal("Go **** yourself and eat some ****!", harmonyStr);
    }

    [Fact]
    public void HarmonizeWordsCaseInsensitive()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go FuCk yourself and eat some shiT!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.Equal("Go **** yourself and eat some ****!", harmonyStr);
    }

    [Fact]
    public void HarmonizeWords_MessedUpSource()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit|"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
        Assert.Equal("Go **** yourself and eat some ****!", harmonyStr);
    }

    [Fact]
    public void ContainsAnyWord_Yes()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go fuck yourself and eat some shit!";
        var b = MaskWordFilter.ContainsAnyWord(disharmonyStr);
        Assert.True(b);
    }

    [Fact]
    public void ContainsAnyWord_No()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("fuck|shit"));

        var disharmonyStr = "Go frack yourself and eat some shirt!";
        var b = MaskWordFilter.ContainsAnyWord(disharmonyStr);
        Assert.False(b);
    }

    // New comprehensive tests for all methods

    #region Constructor Tests

    [Fact]
    public void Constructor_EmptyWordSource_CreatesEmptyFilter()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource(""));

        var result = MaskWordFilter.FilterContent("Any text here");
        Assert.Equal("Any text here", result);
        Assert.False(MaskWordFilter.ContainsAnyWord("Any text here"));
    }

    [Fact]
    public void Constructor_SingleWord_CreatesValidFilter()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.True(MaskWordFilter.ContainsAnyWord("This is bad"));
        Assert.Equal("This is ***", MaskWordFilter.FilterContent("This is bad"));
    }

    [Fact]
    public void Constructor_MultipleWords_CreatesValidFilter()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil|wrong"));

        Assert.True(MaskWordFilter.ContainsAnyWord("This is bad and evil"));
        Assert.Equal("This is *** and ****", MaskWordFilter.FilterContent("This is bad and evil"));
    }

    [Fact]
    public void Constructor_WordsWithDifferentCases_ConvertsToLowerCase()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("BAD|Evil|WrOnG"));

        Assert.True(MaskWordFilter.ContainsAnyWord("This is bad and evil and wrong"));
        Assert.Equal("This is *** and **** and *****", MaskWordFilter.FilterContent("This is bad and evil and wrong"));
    }

    #endregion

    #region AddWord Tests

    [Fact]
    public void AddWord_SingleCharacter_AddsCorrectly()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        addWordMethod.Invoke(filter, new object[] { "a" });

        Assert.True(filter.ContainsAnyWord("a"));
        Assert.Equal("*", filter.FilterContent("a"));
    }

    [Fact]
    public void AddWord_MultipleCharacters_AddsCorrectly()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        addWordMethod.Invoke(filter, new object[] { "test" });

        Assert.True(filter.ContainsAnyWord("This is a test"));
        Assert.Equal("This is a ****", filter.FilterContent("This is a test"));
    }

    [Fact]
    public void AddWord_EmptyString_DoesNotCrash()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        // Should not throw exception
        addWordMethod.Invoke(filter, new object[] { "" });

        Assert.False(filter.ContainsAnyWord("Any text"));
        Assert.Equal("Any text", filter.FilterContent("Any text"));
    }

    [Fact]
    public void AddWord_DuplicateWord_HandlesCorrectly()
    {
        var filter = new TrieTreeWordFilter(new StringWordSource(""));
        var addWordMethod = typeof(TrieTreeWordFilter).GetMethod("AddWord");

        addWordMethod.Invoke(filter, new object[] { "test" });
        addWordMethod.Invoke(filter, new object[] { "test" }); // Add same word again

        Assert.True(filter.ContainsAnyWord("This is a test"));
        Assert.Equal("This is a ****", filter.FilterContent("This is a test"));
    }

    #endregion

    #region ContainsAnyWord Tests

    [Fact]
    public void ContainsAnyWord_EmptyContent_ReturnsFalse()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.False(MaskWordFilter.ContainsAnyWord(""));
    }

    [Fact]
    public void ContainsAnyWord_ContentWithoutBadWords_ReturnsFalse()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.False(MaskWordFilter.ContainsAnyWord("This is a good text"));
    }

    [Fact]
    public void ContainsAnyWord_ContentWithBadWordAtStart_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.True(MaskWordFilter.ContainsAnyWord("bad text here"));
    }

    [Fact]
    public void ContainsAnyWord_ContentWithBadWordInMiddle_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.True(MaskWordFilter.ContainsAnyWord("This bad text"));
    }

    [Fact]
    public void ContainsAnyWord_ContentWithMultipleBadWords_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.True(MaskWordFilter.ContainsAnyWord("This bad and evil text"));
    }

    [Fact]
    public void ContainsAnyWord_CaseInsensitive_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.True(MaskWordFilter.ContainsAnyWord("This BAD text"));
        Assert.True(MaskWordFilter.ContainsAnyWord("This Evil text"));
        Assert.True(MaskWordFilter.ContainsAnyWord("This BaD text"));
    }

    [Fact]
    public void ContainsAnyWord_SingleCharacterWord_ReturnsTrue()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("a|x"));

        Assert.True(MaskWordFilter.ContainsAnyWord("a"));
        Assert.True(MaskWordFilter.ContainsAnyWord("x"));
        Assert.True(MaskWordFilter.ContainsAnyWord("This is a test"));
    }

    #endregion

    #region FilterContent Tests

    [Fact]
    public void FilterContent_EmptyContent_ReturnsEmpty()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.Equal("", MaskWordFilter.FilterContent(""));
    }

    [Fact]
    public void FilterContent_ContentWithoutBadWords_ReturnsOriginal()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        var content = "This is a good text";
        Assert.Equal(content, MaskWordFilter.FilterContent(content));
    }

    [Fact]
    public void FilterContent_ContentWithSingleBadWord_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("This *** text", MaskWordFilter.FilterContent("This bad text"));
    }

    [Fact]
    public void FilterContent_ContentWithMultipleBadWords_FiltersAll()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil|wrong"));

        Assert.Equal("This *** and **** and ***** text",
            MaskWordFilter.FilterContent("This bad and evil and wrong text"));
    }

    [Fact]
    public void FilterContent_BadWordAtStart_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("*** text here", MaskWordFilter.FilterContent("bad text here"));
    }

    [Fact]
    public void FilterContent_BadWordAtEnd_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("This text is ***", MaskWordFilter.FilterContent("This text is bad"));
    }

    [Fact]
    public void FilterContent_ConsecutiveBadWords_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad|evil"));

        Assert.Equal("******* text", MaskWordFilter.FilterContent("badevil text"));
    }

    [Fact]
    public void FilterContent_CaseInsensitive_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("*** *** ***", MaskWordFilter.FilterContent("BAD Bad bAd"));
    }

    [Fact]
    public void FilterContent_SingleCharacterWord_FiltersCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("a|x"));

        Assert.Equal("* test * here", MaskWordFilter.FilterContent("a test x here"));
    }

    [Fact]
    public void FilterContent_OverlappingWords_FiltersLongestMatch()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("test|testing"));

        Assert.Equal("*******", MaskWordFilter.FilterContent("testing"));
        Assert.Equal("****", MaskWordFilter.FilterContent("test"));
    }

    [Fact]
    public void FilterContent_SpecialCharacters_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("This is *** stuff!", MaskWordFilter.FilterContent("This is bad stuff!"));
        Assert.Equal("*** & good", MaskWordFilter.FilterContent("bad & good"));
    }

    [Fact]
    public void FilterContent_NumbersAndLetters_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad123|test"));

        Assert.Equal("****** and ****", MaskWordFilter.FilterContent("bad123 and test"));
    }

    [Fact]
    public void FilterContent_OnlyBadWord_ReturnsOnlyAsterisks()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("***", MaskWordFilter.FilterContent("bad"));
    }

    [Fact]
    public void FilterContent_RepeatedBadWords_FiltersAll()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("*** *** ***", MaskWordFilter.FilterContent("bad bad bad"));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void FilterContent_VeryLongContent_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        var longContent = new string('a', 1000) + " bad " + new string('b', 1000);
        var expected = new string('a', 1000) + " *** " + new string('b', 1000);

        Assert.Equal(expected, MaskWordFilter.FilterContent(longContent));
    }

    [Fact]
    public void ContainsAnyWord_VeryLongContent_HandlesCorrectly()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        var longContent = new string('a', 1000) + " bad " + new string('b', 1000);

        Assert.True(MaskWordFilter.ContainsAnyWord(longContent));
    }

    [Fact]
    public void FilterContent_WhitespaceOnly_ReturnsOriginal()
    {
        MaskWordFilter = new TrieTreeWordFilter(new StringWordSource("bad"));

        Assert.Equal("   ", MaskWordFilter.FilterContent("   "));
        Assert.Equal("\t\n\r", MaskWordFilter.FilterContent("\t\n\r"));
    }

    #endregion
}
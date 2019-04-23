using NUnit.Framework;

namespace Edi.WordFilter.Tests
{
    public class Tests
    {
        public IMaskWordFilter MaskWordFilter { get; set; }

        [SetUp]
        public void Setup()
        {
            MaskWordFilter = new MaskWordFilter(new StringWordSource("fuck|shit"));
        }

        [Test]
        public void TestHarmonizeWords()
        {
            var disharmonyStr = "Go fuck yourself and eat some shit!";
            var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
            Assert.AreEqual("Go **** yourself and eat some ****!", harmonyStr);
        }
    }
}
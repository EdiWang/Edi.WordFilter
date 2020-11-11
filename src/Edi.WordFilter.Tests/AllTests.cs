using NUnit.Framework;

namespace Edi.WordFilter.Tests
{
    [TestFixture]
    public class Tests
    {
        public IMaskWordFilter MaskWordFilter { get; set; }

        [Test]
        public void TestHarmonizeWords()
        {
            MaskWordFilter = new MaskWordFilter(new StringWordSource("fuck|shit"));

            var disharmonyStr = "Go fuck yourself and eat some shit!";
            var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
            Assert.AreEqual("Go **** yourself and eat some ****!", harmonyStr);
        }

        [Test]
        public void TestHarmonizeWordsMessedUpSource()
        {
            MaskWordFilter = new MaskWordFilter(new StringWordSource("fuck|shit|"));

            var disharmonyStr = "Go fuck yourself and eat some shit!";
            var harmonyStr = MaskWordFilter.FilterContent(disharmonyStr);
            Assert.AreEqual("Go **** yourself and eat some ****!", harmonyStr);
        }
    }
}
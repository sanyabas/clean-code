using System.Linq;
using FluentAssertions;
using Markdown;
using NUnit.Framework;

namespace MarkdownTester
{
    [TestFixture]
    public class TokenReaderExtensions_Should
    {
        [Test]
        [TestCase("qweqwe_ qewq", "qweqwe")]
        [TestCase("qweqweq", "qweqweq")]
        public void ExtractSimpleToken(string text, string expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadSimpleToken();
            result.String.Should().Be(expected);
        }

        [Test]
        [TestCase("_qweewq_ qwe", "qweewq")]
        [TestCase("qweqwe", "")]
        public void ExtractItalicTokens_FromSimpleText(string text, string expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadItalicToken();
            result.String.Should().Be(expected);
        }

        [Test]
        [TestCase("qweqwe _wqewqe_ qwe", "qweqwe ", "<em>wqewqe</em>", " qwe")]
        [TestCase("qweqwe", "qweqwe")]
        public void ExtractTokens_FromSimpleText(string text, params string[] expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadTokens().Select(x => x.String);
            result.Should().BeEquivalentTo(expected);
        }
    }
}
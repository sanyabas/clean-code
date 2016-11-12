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
            result.Text.Should().Be(expected);
        }

        [Test]
        [TestCase("_qweewq_ qwe", "qweewq")]
        [TestCase("qweqwe", "")]
        public void ExtractItalicTokens_FromSimpleText(string text, string expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadItalicToken();
            result.Text.Should().Be(expected);
        }

        [Test]
        [TestCase(@"\_qweqwe\_", "_qweqwe_")]
        public void ExtractShadedToken(string text, string expected)
        {
            var reader = new TokenReader(text);
            var result = reader.SkipShadedToken();
            result.Text.Should().Be(expected);
        }

        [Test]
        [TestCase("__Strong text__", "Strong text")]
        public void ExtractBoldToken_FromSimpleText(string text, string expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadBoldToken();
            result.Text.Should().Be(expected);
        }

        [Test]
        [TestCase("qweqwe _wqewqe_ qwe", "qweqwe ", "<em>wqewqe</em>", " qwe")]
        [TestCase("qweqwe", "qweqwe")]
        public void ExtractTokens_FromSimpleText(string text, params string[] expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadTokens().Select(x => x.Text);
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        [TestCase(@"This is \_shadowed\_ text", "This is ", "_shadowed_", " text")]
        public void NotExtract_ShadedItalic(string text, params string[] expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadTokens().Select(x => x.Text);
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        [TestCase(@"Very __very__ bold __text__", "Very ", "<strong>very</strong>", " bold ", "<strong>text</strong>")]
        public void ExtractBoldTokens_SimpleCase(string text, params string[] expected)
        {
            var reader= new TokenReader(text);
            var result = reader.ReadTokens().Select(x => x.Text);
            result.Should().BeEquivalentTo(expected);
        }
    }
}
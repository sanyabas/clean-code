using FluentAssertions;
using Markdown;
using NUnit.Framework;

namespace MarkdownTester
{
    [TestFixture]
    public class TokenReader_Should
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
        [TestCase("Simple plain text", "Simple plain text", 0, '_')]
        [TestCase("Simple plain with _ text", "Simple plain with ", 0, '_')]
        [TestCase("Simple plain with _ text and also *", "Simple plain with ", 0, '*','_')]
        [TestCase("Simple plain with _ and also * text", "Simple plain with _ and also ", 0, '*')]
        
        public void ReadUntil_SpecifiedCharacter(string input, string expectedText, int expectedPosition, params char[] stopChars)
        {
            var reader = new TokenReader(input);
            var result = reader.ReadUntil(0, false, stopChars);
            var expected = new Token(expectedText,expectedPosition);
            result.Should().Be(expected);
        }
    }
}
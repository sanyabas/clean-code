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
        public void ExtractScreenedToken(string text, string expected)
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

        [Test]
        public void Extract_AbsoluteHTTPLink()
        {
            var reader=new TokenReader(@"[absolute link](http://google.com)");
            var result = reader.ReadLink();
            var expected = new Token(@"absolute link", 0);
            expected.HtmlAttributes["href"] = "http://google.com";
            result.Should().Be(expected);
        }

        [Test]
        public void Extract_RelativeHTTPLink()
        {
            var reader = new TokenReader(@"[relative link](/src)", "http://google.com");
            var result = reader.ReadLink();
            var expected = new Token("relative link", 0);
            expected.HtmlAttributes["href"] = "http://google.com/src";
            result.Should().Be(expected);
        }

        [Test]
        [TestCase("http://google.com/","search", "http://google.com/search")]
        [TestCase("http://google.com/","/search", "http://google.com/search")]
        [TestCase("http://google.com","/search", "http://google.com/search")]
        [TestCase("http://google.com","search", "http://google.com/search")]
        public void Combine_URLs(string baseUrl, string relative, string expected)
        {
            var reader = new TokenReader(null);
            var result = reader.CombineUrl(baseUrl, relative);
            result.Should().Be(expected);
        }
    }
}
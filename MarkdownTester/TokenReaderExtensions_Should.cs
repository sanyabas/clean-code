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

        [Test]
        [TestCase(@"This is [absolute](http://google.com) link", "This is ", "<a href=\"http://google.com\">absolute</a>", " link")]
        public void ExtractAbsoluteHttpLinks(string text, params string[] expected)
        {
            var reader = new TokenReader(text);
            var result = reader.ReadTokens().Select(x => x.Text);
            result.Should().BeEquivalentTo(expected);
        }
    }
}
using FluentAssertions;
using Markdown;
using NUnit.Framework;

namespace MarkdownTester
{
    [TestFixture]
    public class TokenExtensions_Should
    {
        [Test]
        [TestCase("string", "<em>", @"<em>string</em>")]
        [TestCase("Some text", "<strong>", @"<strong>Some text</strong>")]
        public void SurroundWithHtmlTags(string text, string tag, string expected)
        {
            var token = new Token(text, 0);
            var result = token.SurroundWithHtmlTag(tag);
            result.String.Should().Be(expected);
        }
    }
}
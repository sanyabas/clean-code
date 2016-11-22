using System.Collections.Generic;
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
            result.Text.Should().Be(expected);
        }

        [Test]
        public void TransformAttributes()
        {
            var attributes = new Dictionary<string, string>
            {
                ["href"] = "http://google.com",
                ["class"] = "class",
                ["id"] = "head"
            };
            var result = TokenExtensions.TransformAttributesToStrings(attributes);
            var expected = "href=\"http://google.com\" class=\"class\" id=\"head\"";
            result.Should().Be(expected);
        }
    }
}
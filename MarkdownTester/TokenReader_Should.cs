using FluentAssertions;
using Markdown;
using NUnit.Framework;

namespace MarkdownTester
{
    [TestFixture]
    public class TokenReader_Should
    {
        [Test]
        [TestCase("Simple plain text", "Simple plain text", 0, '_')]
        [TestCase("Simple plain with _ text", "Simple plain with ", 0, '_')]
        [TestCase("Simple plain with _ text and also *", "Simple plain with ", 0, '*','_')]
        [TestCase("Simple plain with _ and also * text", "Simple plain with _ and also ", 0, '*')]
        
        public void ReadUntil_SpecifiedCharacter(string input, string expectedText, int expectedPosition, params char[] stopChars)
        {
            var reader = new TokenReader(input);
            var result = reader.ReadUntil(0, stopChars);
            var expected = new Token(expectedText,expectedPosition);
            result.Should().Be(expected);
        }
    }
}
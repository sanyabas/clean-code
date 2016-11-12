using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using Markdown;
using FluentAssertions;

namespace MarkdownTester
{
    [TestFixture]
    public class Md_ShouldRender
    {

        private Md renderer;

        [SetUp]
        public void SetUp()
        {
            renderer = new Md();
        }

        [Test]
        public void PlainText_WithoutChanges()
        {
            var input = "This is a sample text";
            var result = renderer.RenderToHtml(input);
            result.Should().Be(input);
        }

        [Test]
        public void ItalicText_WithOneGround()
        {
            var input = @"This _should be italic_";
            var expected = @"This <em>should be italic</em>";
            var result = renderer.RenderToHtml(input);
            result.Should().Be(expected);
        }

        [Test]
        public void BoldText()
        {
            var input = @"This text __should be bold__ text";
            var expected = @"This text <strong>should be bold</strong> text";
            var result = renderer.RenderToHtml(input);
            result.Should().Be(expected);
        }

        [Test]
        public void DifferentVariants()
        {
            var input = @"This is a _complex_ __test text__ with \_screening\_ and \_\_double screening\_\_";
            var expected = @"This is a <em>complex</em> <strong>test text</strong> with _screening_ and __double screening__";
            var result = renderer.RenderToHtml(input);
            result.Should().Be(expected);
        }

        [Test]
        public void PerfomanceTest()
        {
            var input = "_italic_ and __bold text__";
            var firstInput = Repeat("_italic_ and __bold text__", 1000);
            for (var i = 1; i < 6000; i+=1000)
            {
                var sw = Stopwatch.StartNew();
                var result = renderer.RenderToHtml(input);
                Console.WriteLine($"{i} -- {sw.Elapsed}");
                input += firstInput;
            }
        }

        private string Repeat(string input, int count)
        {
            var builder = new StringBuilder(input);
            for (var i = 0; i < count; i++)
                builder.Append(input);
            return builder.ToString();
        }
    }
}

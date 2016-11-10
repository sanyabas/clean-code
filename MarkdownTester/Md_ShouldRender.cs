using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Markdown;
using FluentAssertions;
using NUnit.Framework.Constraints;

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
    }
}

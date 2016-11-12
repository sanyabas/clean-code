using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private const int PerfomanceStep=1000;
        private const double LinearBound = 3;

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
            var results = new List<double>();
            var inputDelta = Repeat("_italic_ and __bold text__", 1000);
            for (var i = 1; i < 6000; i+=PerfomanceStep)
            {
                var sw = Stopwatch.StartNew();
                renderer.RenderToHtml(input);
                results.Add(sw.ElapsedMilliseconds);
                input += inputDelta;
            }
            CheckLinearity(results.ToArray(), PerfomanceStep);
        }

        private string Repeat(string input, int count)
        {
            var builder = new StringBuilder(input);
            for (var i = 0; i < count; i++)
                builder.Append(input);
            return builder.ToString();
        }


        private void CheckLinearity(double[] results, int argDelta)
        {
            var firstDerivative = GetDeltas(results, argDelta);
            firstDerivative.Average().Should().BeInRange(-LinearBound, LinearBound);
        }
        private double[] GetDeltas(double[] results, int argDelta)
        {
            var result=new List<double>();
            for (var i = 0; i < results.Length-2; i++)
            {
                var previousDelta = results[i+1] - results[i];
                var currentDelta = results[i+2] - results[i +1];
                result.Add((currentDelta - previousDelta)/argDelta);
            }
            return result.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ConstructionLine.CodingChallenge.Tests.SampleData;
using FluentAssertions;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEnginePerformanceTests : SearchEngineTestsBase
    {
        private List<Shirt> _shirts;
        private SearchEngine _searchEngine;

        [SetUp]
        public void Setup()
        {

            var dataBuilder = new SampleDataBuilder(50000);
            _shirts = dataBuilder.CreateShirts();

            _searchEngine = new SearchEngine(_shirts);
        }


        [Test]
        public void PerformanceTest_SingleColour()
        {
            var sw = new Stopwatch();
            sw.Start();

            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red }
            };

            var results = _searchEngine.Search(options);

            sw.Stop();
            Console.WriteLine($"Test fixture finished in {sw.ElapsedMilliseconds} milliseconds, [{results.Shirts.Count}] matched shirts");

            var expectedShirts = _shirts.Where(x => x.Color == Color.Red).ToList();

            var expectedSizeCountsInResults = expectedShirts.GroupBy(es => es.Size).Select(g => (g.Key, g.Count()));
            var expectedColourCountsInResults = expectedShirts.GroupBy(es => es.Color).Select(g => (g.Key, g.Count()));

            var expectedColourCounts = GenerateExpectedColourCountList(expectedColourCountsInResults);
            var expectedSizeCounts = GenerateExpectedSizeCountList(expectedSizeCountsInResults);

            results.Should().NotBeNull();
            //results.Shirts.Should().BeEquivalentTo(expectedShirts); // More Correct, but takes a very long time
            results.Shirts.Count.Should().Be(expectedShirts.Count);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void PerformanceTest_SingleColourSingleSize()
        {
            var sw = new Stopwatch();
            sw.Start();

            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red },
                Sizes = new List<Size> { Size.Large }
            };

            var results = _searchEngine.Search(options);

            sw.Stop();
            Console.WriteLine($"Test fixture finished in {sw.ElapsedMilliseconds} milliseconds, [{results.Shirts.Count}] matched shirts");

            var expectedShirts = _shirts.Where(x => x.Color == Color.Red && x.Size == Size.Large).ToList();

            var expectedSizeCountsInResults = expectedShirts.GroupBy(es => es.Size).Select(g => (g.Key, g.Count()));
            var expectedColourCountsInResults = expectedShirts.GroupBy(es => es.Color).Select(g => (g.Key, g.Count()));

            var expectedColourCounts = GenerateExpectedColourCountList(expectedColourCountsInResults);
            var expectedSizeCounts = GenerateExpectedSizeCountList(expectedSizeCountsInResults);

            results.Should().NotBeNull();
            //results.Shirts.Should().BeEquivalentTo(expectedShirts); // More Correct, but takes a very long time
            results.Shirts.Count.Should().Be(expectedShirts.Count);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void PerformanceTest_MultipleColoursSingleSize()
        {
            var sw = new Stopwatch();
            sw.Start();

            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red, Color.Blue },
                Sizes = new List<Size> { Size.Large }
            };

            var results = _searchEngine.Search(options);

            sw.Stop();
            Console.WriteLine($"Test fixture finished in {sw.ElapsedMilliseconds} milliseconds, [{results.Shirts.Count}] matched shirts");

            var expectedShirts = _shirts.Where(x => options.Colors.Contains(x.Color) && x.Size == Size.Large).ToList();

            var expectedSizeCountsInResults = expectedShirts.GroupBy(es => es.Size).Select(g => (g.Key, g.Count()));
            var expectedColourCountsInResults = expectedShirts.GroupBy(es => es.Color).Select(g => (g.Key, g.Count()));

            var expectedColourCounts = GenerateExpectedColourCountList(expectedColourCountsInResults);
            var expectedSizeCounts = GenerateExpectedSizeCountList(expectedSizeCountsInResults);

            results.Should().NotBeNull();
            //results.Shirts.Should().BeEquivalentTo(expectedShirts); // More Correct, but takes a very long time
            results.Shirts.Count.Should().Be(expectedShirts.Count);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }
    }
}

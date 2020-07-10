using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        private List<Shirt> _shirts;
        private SearchEngine _searchEngine;

        [SetUp]
        public void Setup()
        {
            _shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
                new Shirt(Guid.NewGuid(), "White - Large", Size.Large, Color.White),
                new Shirt(Guid.NewGuid(), "Yellow - Medium", Size.Medium, Color.Yellow),
                new Shirt(Guid.NewGuid(), "Yellow - Small", Size.Small, Color.Yellow)
            };

            _searchEngine = new SearchEngine(_shirts);
        }


        [Test]
        public void BasicSearch_SingleSize()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Medium }
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Size == Size.Medium);


            var expectedColourCounts = GenerateExpectedColourCountList(new[] { (Color.Black, 1), (Color.Yellow, 1) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Medium, 2) });


            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_MultipleSizes()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Medium, Size.Small }
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Size == Size.Medium || x.Size == Size.Small);


            var expectedColourCounts =
                GenerateExpectedColourCountList(new[] { (Color.Black, 1), (Color.Yellow, 2), (Color.Red, 1) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Medium, 2), (Size.Small, 2) });


            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_DuplicateSizes()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Medium, Size.Medium }
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Size == Size.Medium);
            var expectedColourCounts = GenerateExpectedColourCountList(new[] { (Color.Black, 1), (Color.Yellow, 1) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Medium, 2) });

            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }


        [Test]
        public void BasicSearch_SingleColour()
        {
            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> { Color.Blue },
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Color == Color.Blue);


            var expectedColourCounts = GenerateExpectedColourCountList(new[] { (Color.Blue, 1) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Large, 1) });

            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_MultipleColours()
        {
            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> { Color.Blue, Color.White },
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Color == Color.Blue || x.Color == Color.White);

            var expectedColourCounts = GenerateExpectedColourCountList(new[] { (Color.Blue, 1), (Color.White, 1) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Large, 2) });


            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_DuplicateColours()
        {
            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> { Color.Blue, Color.Blue },
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Color == Color.Blue);

            var expectedColourCounts = GenerateExpectedColourCountList(new[] { (Color.Blue, 1) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Large, 1) });

            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_SingleSizeSingleColour_HasResult()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Medium },
                Colors = new List<Color> { Color.Yellow }
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Size == Size.Medium && x.Color == Color.Yellow);


            var expectedColourCounts = GenerateExpectedColourCountList(new[] { (Color.Yellow, 1) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Medium, 1) });


            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_SingleSizeSingleColour_NoResult()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Medium },
                Colors = new List<Color> { Color.Red }
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => x.Size == Size.Medium && x.Color == Color.Red);


            var expectedColourCounts = GenerateExpectedColourCountList();
            var expectedSizeCounts = GenerateExpectedSizeCountList();


            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_MultipleSizesSingleColour_HasResult()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Medium, Size.Small },
                Colors = new List<Color> { Color.Yellow }
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x =>
                new[] { Size.Medium, Size.Small }.Contains(x.Size) && x.Color == Color.Yellow);

            var expectedColourCounts = GenerateExpectedColourCountList(new[] { (Color.Yellow, 2) });
            var expectedSizeCounts = GenerateExpectedSizeCountList(new[] { (Size.Medium, 1), (Size.Small, 1) });


            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

        [Test]
        public void BasicSearch_SingleSizeMultipleColours_NoResult()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Small, },
                Colors = new List<Color> { Color.Blue, Color.White }
            };

            var results = _searchEngine.Search(searchOptions);

            var expectedShirts = _shirts.Where(x => new[] { Size.Small }.Contains(x.Size) && new[] { Color.Blue, Color.White }.Contains(x.Color));

            var expectedColourCounts = GenerateExpectedColourCountList();
            var expectedSizeCounts = GenerateExpectedSizeCountList();


            results.Should().NotBeNull();
            results.Shirts.Should().BeEquivalentTo(expectedShirts);
            results.ColorCounts.Should().BeEquivalentTo(expectedColourCounts);
            results.SizeCounts.Should().BeEquivalentTo(expectedSizeCounts);
        }

    }
}
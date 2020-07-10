using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<(Color Colour, List<(Size Size, List<Shirt>)> Items)> _colourCombinationIndex;
        private readonly List<(Size Size, List<(Color Colour, List<Shirt>)> Items)> _sizeCombinationIndex;

        private readonly IEnumerable<int> _quickCombinationHash;

        public SearchEngine(List<Shirt> shirts)
        {
            if (shirts == null) throw new ArgumentNullException(nameof(shirts));

            _colourCombinationIndex = GenerateColourCombinationIndex(shirts);
            _sizeCombinationIndex = GenerateSizeCombinationIndex(shirts);

            var colours = shirts.Select(s => s.Color).Distinct();
            var sizes = shirts.Select(s => s.Size).Distinct();

            var combinations = GenerateColourSizeCombinations(colours, sizes).ToList();
            //combinations.AddRange(Color.All.Select(c => ((Color, Size))(c, null)));
            //combinations.AddRange(Size.All.Select(s => ((Color, Size))(null, s)));

            _quickCombinationHash = combinations.Select(x => x.GetHashCode());
        }


        public SearchResults Search(SearchOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var shirtMatches = new List<Shirt>();

            var colours = options.Colors?.Distinct().ToList();
            var sizes = options.Sizes?.Distinct().ToList();

            var hasColours = colours != null && colours.Any();
            var hasSizes = sizes != null && sizes.Any();

            if (hasColours && hasSizes)
            {
                var optionCombinations = GenerateColourSizeCombinations(colours, sizes);
                var validOptionCombinations = optionCombinations
                    .Where(oc => _quickCombinationHash.Contains(oc.GetHashCode())).ToList();
                foreach (var (colour, size) in validOptionCombinations)
                {
                    var matches = _colourCombinationIndex
                        .Where(x => x.Colour == colour)
                        .SelectMany(x => x.Items)
                        .Where(sg => sg.Size == size)
                        .SelectMany(sg => sg.Item2);
                    shirtMatches.AddRange(matches);
                }
            }
            else
            {
                if (hasColours)
                {
                    foreach (var colour in colours)
                    {
                        var matches = _colourCombinationIndex
                            .Where(cci => cci.Colour == colour)
                            .SelectMany(sg => sg.Items.SelectMany(sgi => sgi.Item2));
                        shirtMatches.AddRange(matches);
                    }
                }
                else if (hasSizes)
                {
                    foreach (var size in sizes)
                    {
                        var matches = _sizeCombinationIndex
                            .Where(sci => sci.Size == size)
                            .SelectMany(cg => cg.Items.SelectMany(cgi => cgi.Item2));
                        shirtMatches.AddRange(matches);
                    }
                }
            }

            var sizeCounts = shirtMatches
                .GroupBy(x => x.Size)
                .Select(g => new SizeCount {Size = g.Key, Count = g.Count()}).ToList();

            var colourCounts = shirtMatches
                .GroupBy(s => s.Color)
                .Select(g => new ColorCount {Color = g.Key, Count = g.Count()}).ToList();

            sizeCounts.AddRange(Size.All.Except(sizeCounts.Select(sc => sc.Size))
                .Select(s => new SizeCount() {Size = s, Count = 0}));
            colourCounts.AddRange(Color.All.Except(colourCounts.Select(cc => cc.Color))
                .Select(c => new ColorCount() {Color = c, Count = 0}));


            var result = new SearchResults
            {
                Shirts = shirtMatches,
                SizeCounts = sizeCounts.ToList(),
                ColorCounts = colourCounts.ToList()
            };
            return result;
        }

        private static IEnumerable<(Color Colour, Size Size)> GenerateColourSizeCombinations(IEnumerable<Color> colours,
            IEnumerable<Size> sizes)
        {
            var result = new List<(Color, Size)>();

            var colourList = colours.ToList();
            var sizeList = sizes.ToList();

            var hasColours = colourList.Any();
            var hasSizes = sizeList.Any();


            if (hasColours)
            {
                foreach (var colour in colourList)
                {
                    if (hasSizes)
                    {
                        result.AddRange(sizeList.Select(size => (colour, size)));
                    }
                    else
                    {
                        result.Add((colour, null));
                    }
                }
            }
            else
            {
                if (hasSizes)
                {
                    result.AddRange(sizeList.Select(size => ((Color, Size)) (null, size)));
                }
            }

            return result;
        }

        private static List<(Color, List<(Size, List<Shirt>)>)> GenerateColourCombinationIndex(
            IEnumerable<Shirt> shirts)
        {
            if (shirts == null) throw new ArgumentNullException(nameof(shirts));

            return shirts.GroupBy(s => s.Color)
                .Select(g =>
                    {
                        var colour = g.Key;
                        var colourGroup = g.GroupBy(cg => cg.Size).Select(sg =>
                        {
                            var size = sg.Key;
                            var sizeGroup = sg.ToList();
                            return (size, sizeGroup);
                        }).ToList();
                        return (colour, colourGroup);
                    }
                ).ToList();
        }

        private static List<(Size, List<(Color, List<Shirt>)>)> GenerateSizeCombinationIndex(IEnumerable<Shirt> shirts)
        {
            if (shirts == null) throw new ArgumentNullException(nameof(shirts));

            return shirts.GroupBy(s => s.Size)
                .Select(g =>
                    {
                        var size = g.Key;
                        var sizeGroup = g.GroupBy(sg => sg.Color).Select(cg =>
                        {
                            var colour = cg.Key;
                            var colourGroup = cg.ToList();
                            return (colour, colourGroup);
                        }).ToList();
                        return (size, sizeGroup);
                    }
                ).ToList();
        }
    }
}
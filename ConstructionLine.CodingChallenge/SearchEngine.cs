using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly Dictionary<Color, Dictionary<Size, List<Shirt>>> _combinationIndex;
        private readonly IEnumerable<int> _quickCombinationHash;

        public SearchEngine(List<Shirt> shirts)
        {
            if (shirts == null) throw new ArgumentNullException(nameof(shirts));

            _combinationIndex = shirts.GroupBy(s => s.Color)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(s => s.Size).ToDictionary(sg => sg.Key, sg => sg.ToList())
                );

            var colours = shirts.Select(s => s.Color).Distinct();
            var sizes = shirts.Select(s => s.Size).Distinct();
            _quickCombinationHash = GenerateColourSizeCombinations(colours, sizes).Select(x => x.GetHashCode());
        }


        public SearchResults Search(SearchOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var optionCombinations = GenerateColourSizeCombinations(options.Colors.Distinct(), options.Sizes.Distinct());
            var validOptionCombinations = optionCombinations.Where(oc => _quickCombinationHash.Contains(oc.GetHashCode()));


            var shirtMatches = new List<Shirt>();
            foreach (var (colour, size) in validOptionCombinations)
            {
                shirtMatches.AddRange(_combinationIndex[colour][size]);
            }

            var sizeCounts = shirtMatches.GroupBy(x => x.Size).Select(g => new SizeCount {Size = g.Key, Count = g.Count()});
            var colourCounts = shirtMatches.GroupBy(s => s.Color).Select(g => new ColorCount {Color = g.Key, Count = g.Count()});

            var result = new SearchResults
            {
                Shirts = shirtMatches,
                SizeCounts = sizeCounts.ToList(),
                ColorCounts = colourCounts.ToList()
            };
            return result;
        }

        private static IEnumerable<(Color Colour, Size Size)> GenerateColourSizeCombinations(IEnumerable<Color> colours, IEnumerable<Size> sizes)
        {
            return (from colour in colours from size in sizes select (colour, size)).ToList();
        }
    }
}
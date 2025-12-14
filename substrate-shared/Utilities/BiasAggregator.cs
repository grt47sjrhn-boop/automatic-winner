using System.Collections.Generic;
using System.Linq;
using substrate_shared.structs;
using substrate_shared.Registries.enums;

namespace substrate_shared.Utilities
{
    public static class BiasAggregator
    {
        public static string DescribeBias(BiasDescriptor descriptor)
        {
            return $"{descriptor.Bias} ({descriptor.Severity}) → {descriptor.Narrative}";
        }

        public static string AggregateBias(IEnumerable<BiasDescriptor> descriptors)
        {
            var list = descriptors.ToList();
            if (!list.Any()) return "No duels in epoch";

            var positive = list.Count(d => d.Bias == Bias.Positive);
            var negative = list.Count(d => d.Bias == Bias.Negative);
            var neutral  = list.Count(d => d.Bias == Bias.Neutral);
            var mixed    = list.Count(d => d.Bias == Bias.Mixed);

            var avgValue = list.Average(d => d.Value);
            var dominantSeverity = list
                .GroupBy(d => d.Severity)
                .OrderByDescending(g => g.Count())
                .First().Key;

            // 🔹 Aggregate facet distributions across all descriptors
            var facetTotals = new Dictionary<ToneType, int>();
            foreach (var desc in list)
            {
                foreach (var facet in desc.Facets)
                {
                    if (!facetTotals.ContainsKey(facet.Key))
                        facetTotals[facet.Key] = 0;
                    facetTotals[facet.Key] += facet.Value;
                }
            }

            var facetSummary = facetTotals.Any()
                ? string.Join(", ", facetTotals.Select(f => $"{f.Key}={f.Value}"))
                : "none";

            return $"Epoch Bias → Pos={positive}, Neg={negative}, Neu={neutral}, Mix={mixed}, " +
                   $"AvgValue={avgValue:F2}, DominantSeverity={dominantSeverity}, Facets=[{facetSummary}]";
        }
    }
}
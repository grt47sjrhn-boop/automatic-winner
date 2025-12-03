using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_shared.types.Summaries
{
    public class ToneClusterSummary : ISummary
    {
        public string Name => nameof(ToneClusterSummary);

        // Core tone states across the pipeline (NarrativeTone-centric)
        public NarrativeTone Baseline { get; set; }
        public NarrativeTone Blended { get; set; }
        public NarrativeTone Complement { get; set; }
        public NarrativeTone BiasAdjusted { get; set; }
        public NarrativeTone FinalTone { get; set; }

        // Neighborhoods derived via canonical enum key but returned as NarrativeTone variants
        public IEnumerable<NarrativeTone> AdjacentTones { get; set; } = Enumerable.Empty<NarrativeTone>();
        public IEnumerable<NarrativeTone> ComplementaryTones { get; set; } = Enumerable.Empty<NarrativeTone>();
        public IEnumerable<NarrativeTone> ClusterNeighborhood { get; set; } = Enumerable.Empty<NarrativeTone>();

        // Angular slices for contributor-facing context
        public IEnumerable<AngularCategoryInfo> AngularCategories { get; set; } = Enumerable.Empty<AngularCategoryInfo>();

        // Category + affinity
        public string Category { get; set; } = "none";
        public TraitAffinity? ResolvedAffinity { get; set; }

        // Registry weights for narratability (keyed by NarrativeTone)
        public IReadOnlyDictionary<NarrativeTone, float> ClusterWeights { get; set; }

        // Trace logs for contributors
        public List<string> TraceLogs { get; set; } = new();

        // Tick context
        public int TickId { get; set; }

        public string Describe()
        {
            var weights = ClusterWeights != null && ClusterWeights.Any()
                ? string.Join(", ", ClusterWeights.Select(kv => $"{kv.Key.Label}={kv.Value:F2}"))
                : "none";

            var traces = TraceLogs != null && TraceLogs.Any()
                ? string.Join(" | ", TraceLogs)
                : "no traces";

            string ToneText(NarrativeTone t) =>
                t == null ? "none" : $"{t.Label}({t.Category},{t.BiasValue})";

            var adjacent = AdjacentTones != null && AdjacentTones.Any()
                ? string.Join(", ", AdjacentTones.Select(ToneText))
                : "none";

            var neighborhood = ClusterNeighborhood != null && ClusterNeighborhood.Any()
                ? string.Join(", ", ClusterNeighborhood.Select(ToneText))
                : "none";

            var complementary = ComplementaryTones != null && ComplementaryTones.Any()
                ? string.Join(", ", ComplementaryTones.Select(ToneText))
                : "none";

            var slices = AngularCategories != null && AngularCategories.Any()
                ? string.Join(" | ", AngularCategories.Select(ac => ac.ToString()))
                : "none";

            var affinityText = ResolvedAffinity.HasValue
                ? $"{ResolvedAffinity.Value} → {ResolvedAffinity.Value.Describe()}"
                : "none";

            return $"ToneClusterSummary [Tick {TickId}] " +
                   $"Baseline={ToneText(Baseline)}, Blended={ToneText(Blended)}, Complement={ToneText(Complement)}, " +
                   $"BiasAdjusted={ToneText(BiasAdjusted)}, Final={ToneText(FinalTone)}, " +
                   $"Category={Category}, Affinity={affinityText}, " +
                   $"Adjacent[{adjacent}], Neighborhood[{neighborhood}], Complementary[{complementary}], " +
                   $"Slices[{slices}], Weights[{weights}], Traces[{traces}]";
        }
    }
}
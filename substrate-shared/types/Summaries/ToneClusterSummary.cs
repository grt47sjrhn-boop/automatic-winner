using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.structs;

namespace substrate_shared.types.Summaries
{
    public class ToneClusterSummary : ISummary
    {
        public string Name => nameof(ToneClusterSummary);

        // Core tone states across the pipeline
        public Tone Baseline { get; set; }
        public Tone Blended { get; set; }
        public Tone Complement { get; set; }
        public Tone BiasAdjusted { get; set; }
        public Tone FinalTone { get; set; }
        
        // New fields
        public IEnumerable<Tone> AdjacentTones { get; set; } = Enumerable.Empty<Tone>();
        public IEnumerable<Tone> ComplementaryTones { get; set; } = Enumerable.Empty<Tone>();
        public IEnumerable<Tone> ClusterNeighborhood { get; set; } = Enumerable.Empty<Tone>();
        public IEnumerable<AngularCategoryInfo> AngularCategories { get; set; } = Enumerable.Empty<AngularCategoryInfo>();
        public TraitAffinity? ResolvedAffinity { get; set; }

        
        // Registry weights for narratability
        public IReadOnlyDictionary<Tone, float> ClusterWeights { get; set; }

        // Trace logs for contributors
        public List<string> TraceLogs { get; set; } = new();

        // Tick context
        public int TickId { get; set; }

        public string Describe()
        {
            var weights = ClusterWeights != null && ClusterWeights.Any()
                ? string.Join(", ", ClusterWeights.Select(kv => $"{kv.Key}={kv.Value:F2}"))
                : "none";

            var traces = TraceLogs != null && TraceLogs.Any()
                ? string.Join(" | ", TraceLogs)
                : "no traces";

            var adjacent = AdjacentTones != null && AdjacentTones.Any()
                ? string.Join(", ", AdjacentTones)
                : "none";

            var neighborhood = ClusterNeighborhood != null && ClusterNeighborhood.Any()
                ? string.Join(", ", ClusterNeighborhood)
                : "none";

            var complementary = ComplementaryTones != null && ComplementaryTones.Any()
                ? string.Join(", ", ComplementaryTones)
                : "none";

            var slices = AngularCategories != null && AngularCategories.Any()
                ? string.Join(" | ", AngularCategories.Select(ac => ac.ToString()))
                : "none";

            return $"ToneClusterSummary [Tick {TickId}] " +
                   $"Baseline={Baseline}, Blended={Blended}, Complement={Complement}, " +
                   $"BiasAdjusted={BiasAdjusted}, Final={FinalTone}, " +
                   $"Adjacent[{adjacent}], Neighborhood[{neighborhood}], Complementary[{complementary}], " +
                   $"Slices[{slices}], Weights[{weights}], Traces[{traces}]";
        }
    }
}
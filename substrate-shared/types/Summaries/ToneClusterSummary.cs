using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using TraitAffinity = substrate_shared.enums.TraitAffinity;


namespace substrate_shared.types.Summaries
{
    public class ToneClusterSummary : ISummary
    {
        public string Name => nameof(ToneClusterSummary);

        // Core tone states
        public NarrativeTone Baseline { get; set; }               // from angle bias
        public IList<(TraitAffinity Affinity, NarrativeTone Tone)> BaseLineTones { get; set; } 
            = new List<(TraitAffinity, NarrativeTone)>();         // candidates fully resolved

        // Trace logs for contributors
        public List<string> TraceLogs { get; set; } = new();

        // Tick context
        public int TickId { get; set; }

        public string Describe()
        {
            string ToneText((TraitAffinity Affinity, NarrativeTone Tone) t) =>
                t.Tone == null ? "none" : $"{t.Tone.Label}({t.Tone.Category},{t.Affinity},{t.Tone.BiasValue})";

            var finals = BaseLineTones.Any()
                ? string.Join(", ", BaseLineTones.Select(ToneText))
                : "none";

            var baseline = $"{Baseline.Label}({Baseline.Category},{Baseline.BiasValue})";

            var traces = TraceLogs.Any()
                ? string.Join(" | ", TraceLogs)
                : "no traces";

            return $"ToneClusterSummary [Tick {TickId}] " +
                   $"Baseline={baseline}, BaseLineTones=[{finals}], Traces[{traces}]";
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;

namespace substrate_shared.types.Summaries
{
    /// <summary>
    /// Contributor-facing summary of intent action resolution for a tick.
    /// </summary>
    public class IntentActionSummary : ISummary
    {
        public string Name => nameof(IntentActionSummary);

        public int TickId { get; set; }

        // Refactored: use NarrativeTone instead of Tone enum
        public NarrativeTone Tone { get; set; }

        public float Persistence { get; set; }
        public float Volatility { get; set; }
        public float Area { get; set; }
        public bool HasDuality { get; set; }
        public IntentType Intent { get; set; }
        public List<string> TraceLogs { get; set; } = new();

        public string Describe()
        {
            var traces = TraceLogs != null && TraceLogs.Any()
                ? string.Join("; ", TraceLogs)
                : "none";

            string toneText = Tone != null
                ? $"{Tone.Label} (Category={Tone.Category}, Bias={Tone.BiasValue})"
                : "none";

            return $"[IntentActionSummary] Tick={TickId}, Tone={toneText}, " +
                   $"Persistence={Persistence:F2}, Volatility={Volatility:F2}, Area={Area:F2}, " +
                   $"Duality={HasDuality}, Intent={Intent}, Traces={traces}";
        }
    }
}
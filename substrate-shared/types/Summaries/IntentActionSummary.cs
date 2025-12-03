using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.interfaces;

namespace substrate_shared.types.Summaries
{
    /// <summary>
    /// Contributor-facing summary of intent action resolution for a tick.
    /// </summary>
    public class IntentActionSummary : ISummary
    {
        public string Name => nameof(IntentActionSummary);

        public int TickId { get; set; }
        public Tone Tone { get; set; }
        public float Persistence { get; set; }
        public float Volatility { get; set; }
        public float Area { get; set; }
        public bool HasDuality { get; set; }
        public IntentType Intent { get; set; }
        public List<string> TraceLogs { get; set; } = new();

        public string Describe()
        {
            var traces = TraceLogs.Count > 0 ? string.Join("; ", TraceLogs) : "none";
            return $"[IntentActionSummary] Tick={TickId}, Tone={Tone}, Persistence={Persistence:F2}, " +
                   $"Volatility={Volatility:F2}, Area={Area:F2}, Duality={HasDuality}, Intent={Intent}, " +
                   $"Traces={traces}";
        }
    }
}
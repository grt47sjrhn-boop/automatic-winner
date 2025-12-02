using System.Collections.Generic;
using System.Linq;
using System.Text;
using substrate_core.Extensions;
using substrate_shared.types.structs;

namespace substrate_core.Generators
{
    public static class RollingSummaryBuilder
    {
        public static string BuildSummary(IEnumerable<TriggerEvent> events)
        {
            if (events == null || !events.Any())
                return "No events recorded in this cycle.";

            var sb = new StringBuilder();

            // Technical compact table
            sb.AppendLine("=== Event Summary Table ===");
            sb.AppendLine("Type\tDescription\tMag\tScore\tTick\tPersistence\tVolatility");
            foreach (var evt in events)
            {
                sb.AppendLine($"{evt.Type}\t{evt.Description}\t{evt.Magnitude:F2}\t{evt.Score:F2}\t{evt.TickId}\t{evt.Persistence:F2}\t{evt.Volatility:F2}");
            }

            sb.AppendLine();
            sb.AppendLine("=== Mythic Overlay ===");

            // Narrative overlay
            foreach (var evt in events)
            {
                sb.AppendLine(evt.ToNarrativeString());
            }

            return sb.ToString();
        }
    }
}
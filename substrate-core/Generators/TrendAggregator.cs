using System.Collections.Generic;
using System.Linq;
using System.Text;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_core.Generators
{
    public static class TrendAggregator
    {
        public static string BuildTrend(IEnumerable<TriggerEvent> events, int windowSize = 10)
        {
            if (events == null || !events.Any())
                return "No events available for trend analysis.";

            var window = events.OrderByDescending(e => e.TickId)
                               .Take(windowSize)
                               .OrderBy(e => e.TickId)
                               .ToList();

            if (!window.Any())
                return "No events in the specified window.";

            // === Technical metrics ===
            float avgPersistence = window.Average(e => e.Persistence);
            float avgVolatility = window.Average(e => e.Volatility);
            float deltaPersistence = window.Last().Persistence - window.First().Persistence;
            float deltaVolatility = window.Last().Volatility - window.First().Volatility;

            var sb = new StringBuilder();
            sb.AppendLine("=== Rolling Trend Analysis ===");
            sb.AppendLine($"Window size: {windowSize} ticks (from {window.First().TickId} to {window.Last().TickId})");
            sb.AppendLine($"Average Persistence: {avgPersistence:F2}, Average Volatility: {avgVolatility:F2}");
            sb.AppendLine($"Δ Persistence: {deltaPersistence:+0.00;-0.00}, Δ Volatility: {deltaVolatility:+0.00;-0.00}");

            sb.AppendLine();
            sb.AppendLine("=== Mythic Arc ===");

            // === Persistence / Volatility arcs ===
            if (window.Any(e => e.Persistence < 0) && window.Any(e => e.Persistence > 0))
                sb.AppendLine("Persistence oscillates — anchors break and reform across cycles.");
            else if (deltaPersistence > 0)
                sb.AppendLine("Persistence strengthens, anchoring the cycle.");
            else if (deltaPersistence < 0)
                sb.AppendLine("Persistence fades, the anchor loosens.");
            else
                sb.AppendLine("Persistence holds steady, equilibrium endures.");

            if (deltaVolatility > 0)
                sb.AppendLine("Volatility rises, storm surges gather.");
            else if (deltaVolatility < 0)
                sb.AppendLine("Volatility calms, storms dissipate.");
            else
                sb.AppendLine("Volatility remains balanced, the storm is held at bay.");

            // === Event clustering ===
            var dominantType = window.GroupBy(e => e.Type)
                                     .OrderByDescending(g => g.Count())
                                     .First().Key.ToString();
            if (dominantType.Contains("Fragmentation"))
                sb.AppendLine("Fragmentation arcs dominate, fracture deepens.");
            else if (dominantType.Contains("Crystallization"))
                sb.AppendLine("Crystallization arcs dominate, resonance strengthens.");
            else
                sb.AppendLine("Arc balance remains mixed, no single event dominates.");

            // === Volatility spikes ===
            if (window.Any(e => e.Volatility > avgVolatility * 1.5))
                sb.AppendLine("Volatility spikes — storm breaking equilibrium.");

            // === Intent trajectories ===
            var intents = window.GroupBy(e => e.Description.Contains("Amplify") ? IntentType.Amplify :
                                               e.Description.Contains("Stabilize") ? IntentType.Stabilize :
                                               e.Description.Contains("Transform") ? IntentType.Transformation :
                                               IntentType.None)
                                .OrderByDescending(g => g.Count())
                                .ToList();
            if (intents.Any())
            {
                switch (intents.First().Key)
                {
                    case IntentType.Amplify:
                        sb.AppendLine("Intent arcs surge toward Amplify — resonance expands.");
                        break;
                    case IntentType.Stabilize:
                        sb.AppendLine("Intent arcs hold toward Stabilize — equilibrium sought.");
                        break;
                    case IntentType.Transformation:
                        sb.AppendLine("Intent arcs shift toward Transformation — change emerges.");
                        break;
                    default:
                        sb.AppendLine("Intent arcs remain diffuse, no clear trajectory.");
                        break;
                }
            }

            // === Legacy tilt trajectories ===
            var legacyTilt = window.GroupBy(e => e.Description.Contains("ResilientHarmony") ? "ResilientHarmony" :
                                                 e.Description.Contains("FracturedLegacy") ? "FracturedLegacy" :
                                                 e.Description.Contains("Equilibrium") ? "Equilibrium" :
                                                 "None")
                                   .OrderByDescending(g => g.Count())
                                   .First().Key;
            if (legacyTilt == "ResilientHarmony")
                sb.AppendLine("Legacy tilt arcs bind toward ResilientHarmony — resilience anchors the cycle.");
            else if (legacyTilt == "FracturedLegacy")
                sb.AppendLine("Legacy tilt arcs bind toward FracturedLegacy — fracture scars deepen.");
            else if (legacyTilt == "Equilibrium")
                sb.AppendLine("Legacy tilt arcs bind toward Equilibrium — balance steadies the axis.");
            else
                sb.AppendLine("Legacy tilt arcs remain diffuse, no clear legacy dominates.");

            // === Emotional spectrum overlay ===
            if (avgPersistence > 20 && avgVolatility < 5)
                sb.AppendLine("Emotional spectrum tilts toward Joy — stability and harmony resonate.");
            else if (avgPersistence < 0 && avgVolatility > 5)
                sb.AppendLine("Emotional spectrum tilts toward Sadness — anchors broken, storms rage.");
            else if (avgVolatility > 10)
                sb.AppendLine("Emotional spectrum tilts toward Fear — volatility overwhelms equilibrium.");
            else
                sb.AppendLine("Emotional spectrum rests in Neutral — balance holds, undertones whisper quietly.");

            return sb.ToString();
        }
    }
}
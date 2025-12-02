using System.Collections.Generic;
using System.Linq;
using substrate_core.Extensions;
using substrate_core.Libraries;
using substrate_core.Logging;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Generators
{
    public class NarrativeGenerator
    {
        public static string Generate(VectorBias vb, int tickId, NarrativeMode mode = NarrativeMode.Hybrid)
        {
            if (vb.ToneTuple.Equals(default(ToneTuple)))
                return $"[Tick {tickId}] No tonal resolution available.";

            vb.TriggerEvents ??= new List<TriggerEvent>();

            // Map Neutral → Equilibrium
            var toneTuple = vb.ToneTuple;
            if (toneTuple.Primary == Tone.Neutral)
                toneTuple = toneTuple with { Primary = Tone.Equilibrium };

            // ✅ Choose template based on Intent first, fallback to Tone
            string template = vb.Intent != IntentType.None
                ? NarrativeTemplateLibrary.GetTemplate(vb.Intent, tickId)
                : NarrativeTemplateLibrary.GetTemplate(toneTuple, tickId);

            // Deduplicate undertones
            var undertones = new HashSet<string>
            {
                toneTuple.Adjacent1.GetNarrativeName(),
                toneTuple.Adjacent2.GetNarrativeName(),
                toneTuple.Complementary.GetNarrativeName()
            }.ToList();

            var line = template
                .Replace("{Primary}", toneTuple.Primary.GetNarrativeName())
                .Replace("{Adj1}", undertones.ElementAtOrDefault(0) ?? "")
                .Replace("{Adj2}", undertones.ElementAtOrDefault(1) ?? "")
                .Replace("{Complementary}", undertones.ElementAtOrDefault(2) ?? "");

            // Legacy tilt
            if (vb.Legacy != LegacyTraitLock.None && mode != NarrativeMode.TechnicalOnly)
                line += $" Legacy tilt binds toward {vb.Legacy.GetNarrativeName()}.";

            // Trigger events
            if (vb.TriggerEvents.Any())
            {
                EventLog.AddEvents(vb.TriggerEvents);

                if (mode != NarrativeMode.TechnicalOnly)
                {
                    var triggerDescriptions = vb.TriggerEvents.Select(t => t.ToNarrativeString());
                    line += $" Events stirred: {string.Join("; ", triggerDescriptions)}.";
                }

                if (mode != NarrativeMode.NarrativeOnly)
                    line += "\n" + RollingSummaryBuilder.BuildSummary(vb.TriggerEvents);

                if (mode == NarrativeMode.Hybrid)
                {
                    var recentEvents = EventLog.GetRecentEvents(10);
                    line += "\n" + TrendAggregator.BuildTrend(recentEvents, 10);
                }
            }

            // Intent line (already covered by template, but keep explicit crystallization note)
            if (vb.Intent != IntentType.None && mode != NarrativeMode.TechnicalOnly)
                line += $" Intent crystallized as {vb.Intent.GetNarrativeName()}.";

            // Technical metrics
            if (mode != NarrativeMode.NarrativeOnly)
                line += $" Persistence anchors at {vb.Persistence:F2}, volatility surges to {vb.Volatility:F2}.";

            return line;
        }
    }
}
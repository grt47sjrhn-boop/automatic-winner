using System.Collections.Generic;
using System.Linq;
using substrate_core.Extensions;
using substrate_core.Libraries;
using substrate_core.Logging;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Generators
{
    public class NarrativeGenerator
    {
        public static string Generate(VectorBias vb, int tickId, NarrativeMode mode = NarrativeMode.Hybrid)
        {
            // ToneClusterSummary drives tonal resolution
            var toneSummary = vb.Summaries.Values.OfType<ToneClusterSummary>().FirstOrDefault();
            if (toneSummary == null)
                return $"[Tick {tickId}] No tonal resolution available.";

            // Pick primary tone: first candidate in BaseLineTones, fallback to Baseline
            var primaryTuple = toneSummary.BaseLineTones.FirstOrDefault();
            var primaryTone  = primaryTuple.Tone ?? toneSummary.Baseline;

            // Map Neutral → Equilibrium
            if (primaryTone != null &&
                primaryTone.Category.Equals("Neutral", System.StringComparison.OrdinalIgnoreCase))
            {
                primaryTone = new NarrativeTone("Equilibrium", "Neutral", "Neutral")
                {
                    Category = "Equilibrium"
                };
            }

            // Build ToneTuple using available candidates
            var toneTuple = new ToneTuple
            {
                Primary       = primaryTone,
                Adjacent1     = toneSummary.BaseLineTones.ElementAtOrDefault(1).Tone,
                Adjacent2     = toneSummary.BaseLineTones.ElementAtOrDefault(2).Tone,
                Complementary = toneSummary.BaseLineTones.ElementAtOrDefault(3).Tone
            };

            // IntentActionSummary drives intent
            var intentSummary = vb.Summaries.Values.OfType<IntentActionSummary>().FirstOrDefault();
            var intent = intentSummary?.Intent ?? IntentType.None;

            // ✅ Choose template based on Intent first, fallback to ToneTuple
            var template = intent != IntentType.None
                ? NarrativeTemplateLibrary.GetTemplate(intent, tickId)
                : NarrativeTemplateLibrary.GetTemplate(toneTuple, tickId);

            // Deduplicate undertones
            var undertones = new HashSet<string?>
            {
                toneTuple.Adjacent1?.Label,
                toneTuple.Adjacent2?.Label,
                toneTuple.Complementary?.Label
            }.Where(s => !string.IsNullOrEmpty(s)).ToList();

            var line = template
                .Replace("{Primary}", toneTuple.Primary?.Label ?? "")
                .Replace("{Adj1}", undertones.ElementAtOrDefault(0) ?? "")
                .Replace("{Adj2}", undertones.ElementAtOrDefault(1) ?? "")
                .Replace("{Complementary}", undertones.ElementAtOrDefault(2) ?? "");

            // Affinity narration: use affinity from primary tuple
            if (primaryTuple.Affinity != TraitAffinity.None && mode != NarrativeMode.TechnicalOnly)
            {
                line += $" Legacy tilt binds toward {primaryTuple.Affinity.GetNarrativeName()}.";
            }

            // Trigger events (from TriggerSummary)
            var triggerSummary = vb.Summaries.Values.OfType<TriggerSummary>().FirstOrDefault();
            var triggerEvents = triggerSummary?.Events ?? [];

            if (triggerEvents.Any())
            {
                EventLog.AddEvents(triggerEvents);

                if (mode != NarrativeMode.TechnicalOnly)
                {
                    var triggerDescriptions = triggerEvents.Select(t => t.ToNarrativeString());
                    line += $" Events stirred: {string.Join("; ", triggerDescriptions)}.";
                }

                if (mode != NarrativeMode.NarrativeOnly)
                    line += "\n" + RollingSummaryBuilder.BuildSummary(triggerEvents);

                if (mode == NarrativeMode.Hybrid)
                {
                    var recentEvents = EventLog.GetRecentEvents(10);
                    line += "\n" + TrendAggregator.BuildTrend(recentEvents, 10);
                }
            }

            // Intent line (explicit crystallization note)
            if (intent != IntentType.None && mode != NarrativeMode.TechnicalOnly)
                line += $" Intent crystallized as {intent.GetNarrativeName()}.";

            // Technical metrics (from PersistenceSummary + VolatilitySummary)
            var persistenceSummary = vb.Summaries.Values.OfType<PersistenceSummary>().FirstOrDefault();
            var volatilitySummary  = vb.Summaries.Values.OfType<VolatilitySummary>().FirstOrDefault();

            if (mode != NarrativeMode.NarrativeOnly)
            {
                var persistenceVal = persistenceSummary?.Current ?? float.NaN;
                var volatilityVal  = volatilitySummary?.Volatility ?? float.NaN;

                line += $" Persistence anchors at {(float.IsNaN(persistenceVal) ? "NaN" : persistenceVal.ToString("F2"))}, " +
                        $"volatility surges to {(float.IsNaN(volatilityVal) ? "NaN" : volatilityVal.ToString("F2"))}.";
            }

            return line;
        }
    }
}
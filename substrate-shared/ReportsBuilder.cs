using System;
using System.Linq;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Registries.enums;
using substrate_shared.Reports;
using substrate_shared.Summaries;
using substrate_shared.Summaries.Base;
using substrate_shared.Traits.Base;

namespace substrate_shared
{
    public class ReportBuilder
    {
        private readonly IResilienceTracker _tracker;
        private readonly InventoryManager _inventory;

        public ReportBuilder(IResilienceTracker tracker, InventoryManager inventory)
        {
            _tracker   = tracker;
            _inventory = inventory;
        }

        public ResilienceReport BuildReport()
        {
            if (_tracker == null) throw new ArgumentNullException(nameof(_tracker));
            if (_inventory == null) throw new ArgumentNullException(nameof(_inventory));

            var summaries = _tracker.DuelSummaries ?? new List<EventSummary>();

            var report = new ResilienceReport
            {
                DuelCount        = summaries.Count,

                // ✅ Outcome-based resilience index
                ResilienceIndex  = summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Recovery)
                                 - summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Wound),

                // ✅ Outcome counts
                RecoveryCount    = summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Recovery),
                CollapseCount    = summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Collapse),
                WoundCount       = summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Wound),
                ConflictCount    = summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Conflict),
                EquilibriumCount = summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Equilibrium),

                // Math overlay values
                AverageHypotenuse = _tracker.AverageHypotenuse,
                CumulativeArea    = _tracker.CumulativeArea,
                MeanCos           = _tracker.MeanCos,
                MeanSin           = _tracker.MeanSin,
                LogScaledIndex    = _tracker.LogScaledIndex,
                ExpScaledIndex    = _tracker.ExpScaledIndex
            };

            // Aggregate tones, intents, brilliance, bias
            foreach (var summary in summaries.OfType<DuelEventSummary>())
            {
                var proseTone = summary.ResolvedVector.Tone?.Label ?? "Unknown";
                report.ToneCounts[proseTone] = report.ToneCounts.TryGetValue(proseTone, out var count) ? count + 1 : 1;

                var duelLabel = $"{summary.DuelistA.Tone?.Label ?? "Unknown"} + {summary.DuelistB.Tone?.Label ?? "Unknown"}";
                report.ToneLabels[duelLabel] = report.ToneLabels.TryGetValue(duelLabel, out var nCount) ? nCount + 1 : 1;

                var intentName = summary.ResolvedVector.Tone?.BiasValue.ToString() ?? "Unknown";
                report.IntentCounts[intentName] = report.IntentCounts.TryGetValue(intentName, out var iCount) ? iCount + 1 : 1;

                if (summary.Brilliance != null)
                {
                    var primary = summary.Brilliance.Primary.ToString();
                    report.BrillianceCuts[primary] = report.BrillianceCuts.TryGetValue(primary, out var bCount) ? bCount + 1 : 1;
                }

                if (summary.Bias?.Narrative != null)
                    report.BiasSummaries.Add(summary.Bias.Narrative);
            }

            // Crystals
            var crystals = _inventory.GetCrystals()?.ToList() ?? new List<TraitCrystal>();

            foreach (var crystal in crystals)
            {
                var rarity = crystal.RarityTier?.Tier ?? "Unknown";
                report.RarityCounts[rarity] = report.RarityCounts.TryGetValue(rarity, out var count) ? count + 1 : 1;

                report.CrystalNarratives.Add(
                    $"{rarity} crystal forged at {crystal.Threshold}, " +
                    $"facets: Resilient({crystal.Facets[ToneType.Resilient]}), " +
                    $"Harmonious({crystal.Facets[ToneType.Harmonious]}), " +
                    $"Conflict({crystal.Facets[ToneType.Conflict]}), " +
                    $"Radiant({crystal.Facets[ToneType.Radiant]}), modifier: {crystal.ModifierValue}"
                );
            }

            // Group crystals for analytic clarity
            var grouped = crystals
                .GroupBy(c =>
                {
                    var keys = c.Facets?.Keys?
                        .OrderBy(k => k.ToString())
                        .Select(k => k.ToString())
                        .ToArray() ?? Array.Empty<string>();
                    return string.Join("+", keys);
                })
                .Select(g =>
                {
                    var sample = g.First();

                    var maxFacetValues = new Dictionary<string, int>();
                    foreach (var facetKey in sample.Facets.Keys)
                    {
                        maxFacetValues[facetKey.ToString()] = g.Max(c => c.Facets[facetKey]);
                    }

                    return new TraitCrystalGroup
                    {
                        Signature      = g.Key,
                        Count          = g.Count(),
                        DominantTone   = g.Last().GetToneType().ToString() ?? "Unknown",
                        Bias           = g.Last().GetBias().ToString() ?? "Unknown",
                        MinModifier    = g.Min(c => c.ModifierValue),
                        MaxModifier    = g.Max(c => c.ModifierValue),
                        MaxFacetValues = maxFacetValues,
                        Crystals       = g.ToList()
                    };
                })
                .ToList();

            report.CrystalGroups = grouped;
            report.Crystals      = crystals;

            return report;
        }
    }
}
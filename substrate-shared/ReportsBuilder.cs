using System.Linq;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.Reports;
using substrate_shared.Summaries;

namespace substrate_shared
{
    public class ReportBuilder
    {
        private readonly IResilienceTracker _tracker;

        public ReportBuilder(IResilienceTracker tracker)
        {
            _tracker = tracker;
        }

        public ResilienceReport BuildReport()
        {
            var report = new ResilienceReport
            {
                DuelCount        = _tracker.DuelSummaries.Count,
                ResilienceIndex  = _tracker.ResilienceIndex,

                // Narrative counts
                RecoveryCount    = _tracker.DuelSummaries.Count(s => s.Description.Contains("Recovery")),
                CollapseCount    = _tracker.DuelSummaries.Count(s => s.Description.Contains("Collapse")),
                WoundCount       = _tracker.DuelSummaries.Count(s => s.Description.Contains("Wound")),
                ConflictCount    = _tracker.DuelSummaries.Count(s => s.Description.Contains("MixedConflict")),
                EquilibriumCount = _tracker.DuelSummaries.Count(s => s.Description.Contains("Equilibrium")),

                // Math overlay values
                AverageHypotenuse = _tracker.AverageHypotenuse,
                CumulativeArea    = _tracker.CumulativeArea,
                MeanCos           = _tracker.MeanCos,
                MeanSin           = _tracker.MeanSin,
                LogScaledIndex    = _tracker.LogScaledIndex,
                ExpScaledIndex    = _tracker.ExpScaledIndex,

                ToneCounts   = new Dictionary<string,int>(),
                IntentCounts = new Dictionary<string,int>(),
                ToneLabels   = new Dictionary<string,int>(),
                CrystalGroups = new List<TraitCrystalGroup>(),
            };

            // Aggregate tones and intents
            foreach (var summary in _tracker.DuelSummaries.OfType<DuelEventSummary>())
            {
                var proseTone = summary.ResolvedVector?.Tone?.Label ?? "Unknown";
                if (!report.ToneCounts.ContainsKey(proseTone))
                    report.ToneCounts[proseTone] = 0;
                report.ToneCounts[proseTone]++;

                var normalized = $"{summary.DuelistA.Tone.Type}+{summary.DuelistB.Tone.Type}";
                if (!report.ToneLabels.ContainsKey(normalized))
                    report.ToneLabels[normalized] = 0;
                report.ToneLabels[normalized]++;

                var intentName = summary.ResolvedVector?.Tone?.BiasValue.ToString() ?? "Unknown";
                if (!report.IntentCounts.ContainsKey(intentName))
                    report.IntentCounts[intentName] = 0;
                report.IntentCounts[intentName]++;
            }

            // 🔹 Group crystals by facet signature (set of facet keys)
            var grouped = _tracker.Crystals
                .GroupBy(c => string.Join("+", c.Facets.Keys.OrderBy(k => k.ToString())))
                .Select(g =>
                {
                    var sample = g.First();

                    // compute highest facet values across group
                    var maxFacetValues = new Dictionary<string,int>();
                    foreach (var facetKey in sample.Facets.Keys)
                    {
                        maxFacetValues[facetKey.ToString()] = g.Max(c => c.Facets[facetKey]);
                    }

                    return new TraitCrystalGroup
                    {
                        Signature = g.Key,
                        Count = g.Count(),
                        DominantTone = g.Last().GetToneType().ToString(),
                        Bias = g.Last().GetBias().ToString(),
                        MinModifier = g.Min(c => c.ModifierValue),
                        MaxModifier = g.Max(c => c.ModifierValue),
                        MaxFacetValues = maxFacetValues
                    };
                })
                .ToList();

            report.CrystalGroups = grouped;

            return report;
        }
    }
}
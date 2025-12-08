using System.Linq;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.Reports;
using substrate_shared.Summaries;
using substrate_shared.Traits.Base;

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
                ConflictCount    = _tracker.DuelSummaries.Count(s => s.Description.Contains("Conflict")),
                EquilibriumCount = _tracker.DuelSummaries.Count(s => s.Description.Contains("Equilibrium")),

                // Math overlay values
                AverageHypotenuse = _tracker.AverageHypotenuse,
                CumulativeArea    = _tracker.CumulativeArea,
                MeanCos           = _tracker.MeanCos,
                MeanSin           = _tracker.MeanSin,
                LogScaledIndex    = _tracker.LogScaledIndex,
                ExpScaledIndex    = _tracker.ExpScaledIndex,

                ToneCounts        = new Dictionary<string,int>(),
                IntentCounts      = new Dictionary<string,int>(),
                ToneLabels        = new Dictionary<string,int>(),
                BrillianceCuts    = new Dictionary<string,int>(),
                RarityCounts      = new Dictionary<string,int>(),
                CrystalNarratives = new List<string>(),
                BiasSummaries     = new List<string>(),
                CrystalGroups     = new List<TraitCrystalGroup>(),
                Crystals          = new List<TraitCrystal>()
            };

            // Aggregate tones, intents, brilliance, and bias summaries
            foreach (var summary in _tracker.DuelSummaries.OfType<DuelEventSummary>())
            {
                var proseTone = summary.ResolvedVector.Tone?.Label ?? "Unknown";
                if (!report.ToneCounts.ContainsKey(proseTone))
                    report.ToneCounts[proseTone] = 0;
                report.ToneCounts[proseTone]++;

                var normalized = $"{summary.DuelistA.Tone.Type}+{summary.DuelistB.Tone.Type}";
                if (!report.ToneLabels.ContainsKey(normalized))
                    report.ToneLabels[normalized] = 0;
                report.ToneLabels[normalized]++;

                var intentName = summary.ResolvedVector.Tone?.BiasValue.ToString() ?? "Unknown";
                if (!report.IntentCounts.ContainsKey(intentName))
                    report.IntentCounts[intentName] = 0;
                report.IntentCounts[intentName]++;

                // Brilliance cuts
                if (summary.Brilliance != null)
                {
                    var primary = summary.Brilliance.Primary.ToString();
                    if (!report.BrillianceCuts.ContainsKey(primary))
                        report.BrillianceCuts[primary] = 0;
                    report.BrillianceCuts[primary]++;
                }

                // Bias summaries
                if (summary.Bias != null)
                    report.BiasSummaries.Add(summary.Bias.Narrative);
            }

            // Group crystals by facet signature
            var grouped = _tracker.Crystals
                .GroupBy(c => string.Join("+", c.Facets.Keys.OrderBy(k => k.ToString())))
                .Select(g =>
                {
                    var sample = g.First();

                    var maxFacetValues = new Dictionary<string,int>();
                    foreach (var facetKey in sample.Facets.Keys)
                    {
                        maxFacetValues[facetKey.ToString()] = g.Max(c => c.Facets[facetKey]);
                    }

                    // Rarity counts
                    var rarity = sample.Rarity.ToString();
                    if (!report.RarityCounts.ContainsKey(rarity))
                        report.RarityCounts[rarity] = 0;
                    report.RarityCounts[rarity]++;

                    // Crystal narratives
                    report.CrystalNarratives.Add(sample.GetDescription());

                    return new TraitCrystalGroup
                    {
                        Signature     = g.Key,
                        Count         = g.Count(),
                        DominantTone  = g.Last().GetToneType().ToString(),
                        Bias          = g.Last().GetBias().ToString(),
                        MinModifier   = g.Min(c => c.ModifierValue),
                        MaxModifier   = g.Max(c => c.ModifierValue),
                        MaxFacetValues = maxFacetValues,
                        Crystals      = g.ToList()
                    };
                })
                .ToList();

            report.CrystalGroups = grouped;
            report.Crystals      = _tracker.Crystals.ToList();

            return report;
        }
    }
}
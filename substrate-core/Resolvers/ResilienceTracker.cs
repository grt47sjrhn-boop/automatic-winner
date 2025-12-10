using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.Summaries.Types;
using substrate_shared.Traits.Base;
using substrate_shared.Reports;   // ✅ bring in ResilienceReport DTO

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Tracks duel outcomes, overlays, and crystals, then builds ResilienceReport DTOs.
    /// </summary>
    public class ResilienceTracker : IResilienceTracker
    {
        private readonly List<ISummary> _summaries = new();
        private readonly List<TraitCrystal> _crystals = new();
        private readonly List<string> _narratives = new();   // ✅ store enriched narratives

        private readonly List<double> _hypotenuse = new();
        private readonly List<double> _areas = new();
        private readonly List<(double cos, double sin, double log, double exp)> _trig = new();

        private double _totalResilience = 0.0;

        // Public accessors
        public IReadOnlyList<ISummary> DuelSummaries => _summaries;
        public IReadOnlyList<TraitCrystal> Crystals => _crystals;
        public IReadOnlyList<string> Narratives => _narratives;

        public double TotalResilience => _totalResilience;
        public double ResilienceIndex => _summaries.Count > 0
            ? _totalResilience / _summaries.Count
            : 0.0;

        public double AverageHypotenuse => _hypotenuse.Count > 0 ? _hypotenuse.Average() : 0.0;
        public double CumulativeArea => _areas.Sum();
        public double MeanCos => _trig.Count > 0 ? _trig.Average(t => t.cos) : 0.0;
        public double MeanSin => _trig.Count > 0 ? _trig.Average(t => t.sin) : 0.0;
        public double LogScaledIndex => _trig.Count > 0 ? _trig.Average(t => t.log) : 0.0;
        public double ExpScaledIndex => _trig.Count > 0 ? _trig.Average(t => t.exp) : 0.0;

        public ISummary Record(ISummary summary, BiasVector? a = null, BiasVector? b = null)
        {
            AddSummary(summary);
            return summary;
        }

        /// <summary>
        /// Build a full ResilienceReport DTO with aggregate metrics, distributions, and crystal data.
        /// </summary>
        public ResilienceReport ComputeResilience()
        {
            var report = new ResilienceReport
            {
                DuelCount       = _summaries.Count,
                ResilienceIndex = ResilienceIndex,
                TotalResilience = TotalResilience,

                RecoveryCount   = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Recovery),
                CollapseCount   = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Collapse),
                WoundCount      = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Wound),
                ConflictCount   = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Conflict),
                EquilibriumCount= _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Equilibrium),

                AverageHypotenuse = AverageHypotenuse,
                CumulativeArea    = CumulativeArea,
                MeanCos           = MeanCos,
                MeanSin           = MeanSin,
                LogScaledIndex    = LogScaledIndex,
                ExpScaledIndex    = ExpScaledIndex,

                CrystalCount      = _crystals.Count,
                Crystals          = _crystals.ToList(),
                CrystalNarratives = _narratives.ToList()   // ✅ include stored narratives
            };

            // 🔹 Tone distribution
            foreach (var summary in _summaries.OfType<DuelEventSummary>())
            {
                var tone = summary.ResolvedVector.Tone?.Label ?? "Unknown";
                if (!report.ToneDistribution.ContainsKey(tone))
                    report.ToneDistribution[tone] = 0;
                report.ToneDistribution[tone]++;
            }

            // 🔹 Intent distribution
            foreach (var summary in _summaries.OfType<DuelEventSummary>())
            {
                var intent = summary.ResolvedIntent.ToString();
                if (!report.IntentDistribution.ContainsKey(intent))
                    report.IntentDistribution[intent] = 0;
                report.IntentDistribution[intent]++;
            }

            // 🔹 Crystal rarities
            foreach (var crystal in _crystals)
            {
                var rarityKey = crystal.ResolvedRarity.ToString();
                if (report.RarityCounts.ContainsKey(rarityKey))
                    report.RarityCounts[rarityKey]++;
                else
                    report.RarityCounts[rarityKey] = 1;
            }

            // ✅ Mirror into CrystalRarity dictionary for export
            report.CrystalRarity = new Dictionary<string,int>(report.RarityCounts);

            // 🔹 Bias summaries
            foreach (var summary in _summaries.OfType<DuelEventSummary>())
            {
                var bias = $"{summary.Bias.Bias} ({summary.Bias.Value:F2}, {summary.ResolvedRarity})";
                report.BiasSummaries.Add(bias);
            }

            return report;
        }

        // Overlay accumulation
        public void AddHypotenuse(double value) => _hypotenuse.Add(value);
        public void AddArea(double value) => _areas.Add(value);
        public void AddTrig(double cos, double sin, double log, double exp) => _trig.Add((cos, sin, log, exp));

        // Summary + crystal accumulation
        public void AddSummary(ISummary summary) => _summaries.Add(summary);
        public void AddCrystal(TraitCrystal crystal) => _crystals.Add(crystal);

        public List<ISummary> GetSummaries() => new(_summaries);
        public List<TraitCrystal> GetCrystals() => new(_crystals);

        public void AddResilience(double engagementCumulativeResilience)
        {
            _totalResilience += engagementCumulativeResilience;
        }

        /// <summary>
        /// Add a narrative string to the tracker for later inclusion in reports.
        /// </summary>
        public void AddNarrative(string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                _narratives.Add(description);
        }
    }
}
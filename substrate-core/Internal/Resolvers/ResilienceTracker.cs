using System.Collections.Generic;
using System.Linq;
using substrate_core.Models.Summaries;
using substrate_core.Models.Summaries.Types;
using substrate_shared.interfaces;
using substrate_shared.interfaces.core;
using substrate_shared.Registries.enums;
using substrate_shared.Reports;
using substrate_shared.structs;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Types;

namespace substrate_core.Internal.Resolvers
{
    /// <summary>
    /// Tracks duel outcomes, overlays, and crystals, then builds ResilienceReport DTOs.
    /// </summary>
    public class ResilienceTracker : IResilienceTracker
    {
        private readonly List<ISummary> _summaries = new();
        private readonly List<TraitCrystal> _crystals = new();
        private readonly List<string> _narratives = new();

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
        public IResilienceReport ComputeResilience()
        {
            // Aggregate counts
            int duelCount = _summaries.Count;
            double resilienceIndex = ResilienceIndex;
            double totalResilience = TotalResilience;

            int recoveryCount    = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Recovery);
            int collapseCount    = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Collapse);
            int woundCount       = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Wound);
            int conflictCount    = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Conflict);
            int equilibriumCount = _summaries.OfType<DuelEventSummary>().Count(s => s.Outcome == DuelOutcome.Equilibrium);

            // Math overlays
            double avgHypotenuse = AverageHypotenuse;
            double cumulativeArea = CumulativeArea;
            double meanCos = MeanCos;
            double meanSin = MeanSin;
            double logScaledIndex = LogScaledIndex;
            double expScaledIndex = ExpScaledIndex;

            // Tone distribution
            var toneDistribution = new Dictionary<string,int>();
            foreach (var summary in _summaries.OfType<DuelEventSummary>())
            {
                var tone = summary.ResolvedVector.Tone?.Label ?? "Unknown";
                if (!toneDistribution.ContainsKey(tone))
                    toneDistribution[tone] = 0;
                toneDistribution[tone]++;
            }

            // Intent distribution
            var intentDistribution = new Dictionary<string,int>();
            foreach (var summary in _summaries.OfType<DuelEventSummary>())
            {
                var intent = summary.ResolvedIntent.ToString();
                if (!intentDistribution.ContainsKey(intent))
                    intentDistribution[intent] = 0;
                intentDistribution[intent]++;
            }

            // Crystal rarities
            var rarityCounts = new Dictionary<string,int>
            {
                { "Common", 0 }, { "Rare", 0 }, { "Epic", 0 }, { "Mythic", 0 },
                { "Legendary", 0 }, { "UltraRare", 0 }, { "Fragile", 0 },
                { "Corrupted", 0 }, { "Doomed", 0 }
            };
            foreach (var crystal in _crystals)
            {
                var rarityKey = crystal.ResolvedRarity.ToString();
                if (rarityCounts.ContainsKey(rarityKey))
                    rarityCounts[rarityKey]++;
                else
                    rarityCounts[rarityKey] = 1;
            }

            var crystalRarity = new Dictionary<string,int>(rarityCounts);

            // Bias summaries
            var biasSummaries = new List<string>();
            foreach (var summary in _summaries.OfType<DuelEventSummary>())
            {
                var bias = $"{summary.Bias.Bias} ({summary.Bias.Value:F2}, {summary.ResolvedRarity})";
                biasSummaries.Add(bias);
            }
            
            // Build report via SetMetrics
            var report = new ResilienceReport();
            report.SetMetrics(
                duelCount, resilienceIndex, totalResilience,
                recoveryCount, collapseCount, woundCount, conflictCount, equilibriumCount,
                avgHypotenuse, cumulativeArea, meanCos, meanSin,
                logScaledIndex, expScaledIndex,
                toneDistribution,
                intentDistribution,
                CreateCrystalsGroup(),
                _crystals.ToList(),
                rarityCounts,
                _narratives.ToList(),
                biasSummaries,
                _crystals.Count,
                new Dictionary<string,int>(), // Outcomes placeholder
                crystalRarity,
                new Dictionary<string,int>()  // BrillianceCuts placeholder
            );

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
        public IReadOnlyList<TraitCrystalGroup> CrystalGroups => CreateCrystalsGroup();

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

        private List<TraitCrystalGroup> CreateCrystalsGroup()
        {
            var crystalGroups = _crystals
                .GroupBy(c => c.ResolvedRarity.ToString())
                .Select(g => new TraitCrystalGroup
                {
                    Signature = g.Key,
                    Count = g.Count(),

                    // Use the helper on TraitCrystal to determine dominant tone
                    DominantTone = g.GroupBy(c => c.GetToneType().ToString())
                        .OrderByDescending(t => t.Count())
                        .First().Key,

                    // Use the helper on TraitCrystal to determine bias
                    Bias = g.GroupBy(c => c.GetBias().ToString())
                        .OrderByDescending(b => b.Count())
                        .First().Key,

                    MinModifier = g.Min(c => c.ModifierValue),
                    MaxModifier = g.Max(c => c.ModifierValue),

                    Crystals = g.ToList(),

                    MaxFacetValues = g.SelectMany(c => c.Facets)
                        .GroupBy(f => f.Key.ToString())
                        .ToDictionary(f => f.Key, f => f.Max(x => x.Value))
                })
                .ToList();

            return crystalGroups;
        }
    }
}
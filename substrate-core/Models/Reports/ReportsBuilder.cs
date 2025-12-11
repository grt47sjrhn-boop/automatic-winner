using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Managers;
using substrate_shared.interfaces.core;
using substrate_shared.Reports;
using substrate_shared.Traits.Types;
// IResilienceReport

namespace substrate_core.Models.Reports
{
    public class ReportBuilder : IReportBuilder
    {
        private readonly IResilienceTracker _tracker;
        private readonly InventoryManager _inventory;

        public ReportBuilder(IResilienceTracker tracker, InventoryManager inventoryManager)
        {
            _tracker   = tracker ?? throw new ArgumentNullException(nameof(tracker));
            _inventory = inventoryManager ?? throw new ArgumentNullException(nameof(inventoryManager));
        }

        public IResilienceReport BuildReport()
        {
            var summaries = _tracker.GetSummaries();
            var crystals  = _tracker.GetCrystals();

            // --- Aggregate first ---
            var outcomes = summaries
                .GroupBy(s => s.Outcome.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var recoveryCount    = outcomes.GetValueOrDefault("Recovery");
            var collapseCount    = outcomes.GetValueOrDefault("Collapse");
            var woundCount       = outcomes.GetValueOrDefault("Wound");
            var conflictCount    = outcomes.GetValueOrDefault("Conflict");
            var equilibriumCount = outcomes.GetValueOrDefault("Equilibrium");

            var intentDistribution = summaries
                .GroupBy(s => s.Intent.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var toneDistribution = summaries
                .GroupBy(s => s.Brilliance.Primary.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var brillianceCuts = summaries
                .GroupBy(s => s.Brilliance.Primary.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var rarityCounts = new Dictionary<string, int>();
            var crystalNarratives = new List<string>();

            foreach (var crystal in crystals)
            {
                var rarityKey = crystal.ResolvedRarity.ToString();
                rarityCounts[rarityKey] = rarityCounts.GetValueOrDefault(rarityKey) + 1;

                crystalNarratives.Add(
                    $"{rarityKey} crystal forged " +
                    $"(Hypotenuse {_tracker.AverageHypotenuse:F1}, Area {_tracker.CumulativeArea:F1}) " +
                    $"→ Facets: {string.Join(", ", crystal.Facets.Select(f => $"{f.Key}:{f.Value}"))}"
                );
            }

            var crystalRarity = new Dictionary<string, int>(rarityCounts);

            var biasSummaries = summaries
                .OfType<IDuelEventSummary>() // use interface, not concrete
                .GroupBy(s => s.Bias.Bias)
                .Select(g => $"{g.Key}: {g.Count()} duels")
                .ToList();

            var crystalGroups = crystals
                .GroupBy(c => c.GetGroup())
                .Select(g => new TraitCrystalGroup
                {
                    Signature      = g.Key.ToString(),
                    Count          = g.Count(),
                    DominantTone   = g.First().ToneCut?.Primary.ToString() ?? string.Empty,
                    Bias           = g.First().GetBias().ToString() ?? string.Empty,
                    Crystals       = g.ToList(),
                    MaxFacetValues = g.First().Facets.ToDictionary(f => f.Key.ToString(), f => f.Value)
                })
                .ToList();

            // --- Construct concrete, initialize once, return as interface ---
            var report = new ResilienceReport();
            report.SetMetrics(
                duelCount: summaries.Count,
                resilienceIndex: _tracker.ResilienceIndex,
                totalResilience: _tracker.TotalResilience,
                recoveryCount: recoveryCount,
                collapseCount: collapseCount,
                woundCount: woundCount,
                conflictCount: conflictCount,
                equilibriumCount: equilibriumCount,
                avgHypotenuse: _tracker.AverageHypotenuse,
                cumulativeArea: _tracker.CumulativeArea,
                meanCos: _tracker.MeanCos,
                meanSin: _tracker.MeanSin,
                logScaledIndex: _tracker.LogScaledIndex,
                expScaledIndex: _tracker.ExpScaledIndex,
                toneDistribution: toneDistribution,
                intentDistribution: intentDistribution,
                crystalGroups: crystalGroups,
                crystals: crystals,
                rarityCounts: rarityCounts,
                crystalNarratives: crystalNarratives,
                biasSummaries: biasSummaries,
                crystalCount: crystals.Count,
                outcomes: outcomes,
                crystalRarity: crystalRarity,
                brillianceCuts: brillianceCuts
            );

            return report; // IResilienceReport
        }
    }
}
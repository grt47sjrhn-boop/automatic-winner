using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Reports;
using substrate_shared.Summaries;
using substrate_shared.Traits.Base;

namespace substrate_shared
{
    public class ReportBuilder
    {
        private readonly IResilienceTracker _tracker;
        private readonly InventoryManager _inventory;

        public ReportBuilder(IResilienceTracker tracker, InventoryManager inventoryManager)
        {
            _tracker   = tracker ?? throw new ArgumentNullException(nameof(tracker));
            _inventory = inventoryManager ?? throw new ArgumentNullException(nameof(inventoryManager));
        }

        public ResilienceReport BuildReport()
        {
            var summaries = _tracker.GetSummaries();
            var crystals  = _tracker.GetCrystals();

            var report = new ResilienceReport
            {
                DuelCount        = summaries.Count,
                CrystalCount     = crystals.Count,
                AverageHypotenuse= _tracker.AverageHypotenuse,
                CumulativeArea   = _tracker.CumulativeArea,
                MeanCos          = _tracker.MeanCos,
                MeanSin          = _tracker.MeanSin,
                LogScaledIndex   = _tracker.LogScaledIndex,
                ExpScaledIndex   = _tracker.ExpScaledIndex,
                Crystals         = crystals,
                TotalResilience  = _tracker.TotalResilience,
                ResilienceIndex  = _tracker.ResilienceIndex
            };

            // --- Outcome counts ---
            report.Outcomes = summaries
                .GroupBy(s => s.Outcome.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            report.RecoveryCount    = report.Outcomes.GetValueOrDefault("Recovery");
            report.CollapseCount    = report.Outcomes.GetValueOrDefault("Collapse");
            report.WoundCount       = report.Outcomes.GetValueOrDefault("Wound");
            report.ConflictCount    = report.Outcomes.GetValueOrDefault("Conflict");
            report.EquilibriumCount = report.Outcomes.GetValueOrDefault("Equilibrium");

            // --- Intent distribution ---
            report.IntentDistribution = summaries
                .GroupBy(s => s.Intent.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // --- Tone distribution ---
            report.ToneDistribution = summaries
                .GroupBy(s => s.Brilliance.Primary.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // --- Crystal rarity counts + narratives ---
            foreach (var crystal in crystals)
            {
                // ✅ Always use ResolvedRarity consistently
                var rarityKey = crystal.ResolvedRarity.ToString();

                if (report.RarityCounts.ContainsKey(rarityKey))
                    report.RarityCounts[rarityKey]++;
                else
                    report.RarityCounts[rarityKey] = 1;

                // ✅ Enrich narrative with overlay context
                report.CrystalNarratives.Add(
                    $"{rarityKey} crystal forged " +
                    $"(Hypotenuse {_tracker.AverageHypotenuse:F1}, Area {_tracker.CumulativeArea:F1}) " +
                    $"→ Facets: {string.Join(", ", crystal.Facets.Select(f => $"{f.Key}:{f.Value}"))}"
                );
            }

            // ✅ Mirror into CrystalRarity dictionary for export
            report.CrystalRarity = new Dictionary<string,int>(report.RarityCounts);


            // --- Crystal narratives ---
            report.CrystalNarratives = crystals
                .Select(c => c.GetDescription())
                .ToList();

            // --- Bias summaries ---
            report.BiasSummaries = summaries
                .OfType<DuelEventSummary>()
                .GroupBy(s => s.Bias.Bias)
                .Select(g => $"{g.Key}: {g.Count()} duels")
                .ToList();

            // --- Brilliance cuts ---
            report.BrillianceCuts = summaries
                .GroupBy(s => s.Brilliance.Primary.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // --- Crystal groups ---
            report.CrystalGroups = crystals
                .GroupBy(c => c.GetGroup())
                .Select(g => new TraitCrystalGroup
                {
                    Signature    = g.Key.ToString(),
                    Count        = g.Count(),
                    DominantTone = g.First().ToneCut?.Primary.ToString() ?? string.Empty,
                    Bias         = g.First().GetBias().ToString() ?? string.Empty,
                    Crystals     = g.ToList(),
                    MaxFacetValues = g.First().Facets.ToDictionary(f => f.Key.ToString(), f => f.Value)
                })
                .ToList();

            return report;
        }
    }
}
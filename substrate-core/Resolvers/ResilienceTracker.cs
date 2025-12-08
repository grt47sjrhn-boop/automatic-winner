using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Managers;
using substrate_core.Summaries.Types;
using substrate_shared.interfaces;
using substrate_shared.types;
using substrate_shared.Overlays;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.Factories;
using substrate_shared.Registries.Managers;
using substrate_shared.Summaries.Base;
using substrate_shared.Traits.Base;

namespace substrate_core.Resolvers
{
    public class ResilienceTracker : IResilienceTracker
    {
        private readonly List<ISummary> _duelSummaries = new();
        private readonly List<(BiasVector, BiasVector)> _duelPairs = new();
        private readonly List<TraitCrystal> _crystals = new();   // 🔹 Crystal inventory
        private int _resilienceIndex = 0;

        public IReadOnlyList<ISummary> DuelSummaries => _duelSummaries;
        public IReadOnlyList<TraitCrystal> Crystals => _crystals;
        public int ResilienceIndex => _resilienceIndex;

        // 🔹 Math overlay properties exposed via interface
        public double AverageHypotenuse => TrigOverlay.AverageHypotenuse(_duelPairs);
        public double CumulativeArea => TrigOverlay.CumulativeArea(_duelPairs);
        public double MeanCos => TrigOverlay.MeanCos(_duelPairs);
        public double MeanSin => TrigOverlay.MeanSin(_duelPairs);
        public double LogScaledIndex => TrigOverlay.LogScaledIndex(_resilienceIndex);
        public double ExpScaledIndex => TrigOverlay.ExpScaledIndex(_resilienceIndex);

        // Record duel summary and optional vector pair for math overlay
        public ISummary Record(ISummary summary, BiasVector? a = null, BiasVector? b = null)
        {
            _duelSummaries.Add(summary);

            if (summary is DuelEventSummary duelSummary)
            {
                // 🔹 Update resilience index based on outcome
                switch (duelSummary.Outcome)
                {
                    case DuelOutcome.Recovery:    _resilienceIndex += 2; break;
                    case DuelOutcome.Collapse:    _resilienceIndex -= 2; break;
                    case DuelOutcome.Wound:       _resilienceIndex -= 1; break;
                    case DuelOutcome.Conflict:    _resilienceIndex -= 1; break;
                    case DuelOutcome.Equilibrium: _resilienceIndex += 1; break;
                }

                // 🔹 Crystal forging check (mod-6 rule, multiple crystals if needed)
                while (Math.Abs(_resilienceIndex) >= 6)
                {
                    var facets = CollectFacets();
                    var narrative = SuperRegistryManager.DescribeClusterWithScore(NarrativeGroup.Crystal);

                    // 🔹 Build tone cut from facets
                    var toneCut = ToneManager.Cut(facets);

                    // 🔹 Assign rarity tier from resilience score
                    var rarityTier = RarityManager.AssignTier(_resilienceIndex);

                    // 🔹 Create crystal with full parameter set
                    var crystal = TraitCrystalFactory.CreateCrystal(
                        threshold: _resilienceIndex > 0 ? 6 : -6,
                        isPositive: _resilienceIndex > 0,
                        facets: facets,
                        narrative: narrative,
                        existingCrystals: _crystals,
                        toneCut: toneCut,
                        rarityTier: rarityTier
                    );


                    _crystals.Add(crystal);

                    // carry remainder by subtracting/adding 6 until back in range
                    _resilienceIndex = _resilienceIndex > 0
                        ? _resilienceIndex - 6
                        : _resilienceIndex + 6;
                }
            }

            if (a.HasValue && b.HasValue)
                _duelPairs.Add((a.Value, b.Value));

            return BuildOverlay();
        }

        // 🔹 Collect facets from duel summaries
        private IReadOnlyDictionary<ToneType,int> CollectFacets()
        {
            var facetCounts = new Dictionary<ToneType,int>();

            foreach (var summary in _duelSummaries.OfType<DuelEventSummary>())
            {
                // Map duel outcome → tone facet
                var tone = summary.Outcome switch
                {
                    DuelOutcome.Recovery    => ToneType.Joy,
                    DuelOutcome.Collapse    => ToneType.Despairing,
                    DuelOutcome.Wound       => ToneType.Wound,
                    DuelOutcome.Conflict    => ToneType.Conflict,
                    DuelOutcome.Equilibrium => ToneType.Equilibrium,
                    _ => ToneType.Neutral
                };

                facetCounts.TryAdd(tone, 0);

                facetCounts[tone]++;
            }

            return facetCounts;
        }

        private ISummary BuildOverlay()
        {
            var recoveries = _duelSummaries.Count(s => s.Description.Contains("Recovery"));
            var collapses  = _duelSummaries.Count(s => s.Description.Contains("Collapse"));
            var wounds     = _duelSummaries.Count(s => s.Description.Contains("Wound"));
            var conflicts  = _duelSummaries.Count(s => s.Description.Contains("MixedConflict"));

            var crystalSummary = _crystals.Any()
                ? $"Crystals forged: {_crystals.Count} → " +
                  string.Join(", ", _crystals.Select(c => $"{c.Rarity} {c.Type}"))
                : "No crystals forged yet.";

            var description =
                $"Trajectory: {recoveries} recoveries, {collapses} collapses, {wounds} wounds, {conflicts} conflicts. " +
                $"Resilience Index: {_resilienceIndex}. " +
                $"Math Overlay → AvgHyp: {AverageHypotenuse:F2}, CumArea: {CumulativeArea:F2}, " +
                $"MeanCos: {MeanCos:F2}, MeanSin: {MeanSin:F2}, " +
                $"LogIndex: {LogScaledIndex:F2}, ExpIndex: {ExpScaledIndex:F2}. " +
                crystalSummary;

            return new EventSummary(
                $"Resilience Overlay (after {_duelSummaries.Count} duel{(_duelSummaries.Count == 1 ? "" : "s")})",
                description,
                SummaryType.System,
                false
            );
        }

        public ISummary ComputeResilience()
        {
            if (!_duelSummaries.Any())
                return new EventSummary("Resilience Report", "No duels recorded yet.", SummaryType.System, false);

            string description;
            var resolved = false;

            if (Math.Abs(_resilienceIndex) >= 6)
            {
                description = _resilienceIndex > 0
                    ? $"System crystallizes with resilience index {_resilienceIndex}."
                    : $"System collapses into fragments with resilience index {_resilienceIndex}.";
                resolved = true;
            }
            else
            {
                description = $"System remains oscillating with resilience index {_resilienceIndex}.";
            }

            return new EventSummary("Resilience Report", description, SummaryType.System, resolved);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Factories;
using substrate_core.Managers;
using substrate_core.Models;
using substrate_core.Models.Engagements.Enums;
using substrate_core.Runners.Factories;
using substrate_shared.Enums;
using substrate_shared.interfaces;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Managers;
using substrate_shared.interfaces.Overlays; // 🔹 for IGeometryOverlay, ITrigOverlay
using substrate_shared.Profiles;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
// EngagementType enum
using EnvironmentMood = substrate_shared.Enums.EnvironmentMood;

namespace substrate_core.Internal.Engines
{
    /// <summary>
    /// Engine that runs duels for a persistent duelist and records outcomes.
    /// </summary>
    public class DuelEngine
    {
        private readonly double _difficulty;
        private readonly IResilienceTracker _tracker;
        private readonly Duelist _persistent;
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;
        private readonly InventoryManager _inventory;

        // 🔹 Overlays injected
        private readonly IGeometryOverlay _geometryOverlay;
        private readonly ITrigOverlay _trigOverlay;

        public DuelEngine(
            IResilienceTracker tracker,
            Duelist persistent,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            InventoryManager inventory,
            IGeometryOverlay geometryOverlay,
            ITrigOverlay trigOverlay,
            double difficulty = 1.0)
        {
            _tracker        = tracker;
            _persistent     = persistent;
            _biasManager    = biasManager;
            _facetManager   = facetManager;
            _toneManager    = toneManager;
            _rarityManager  = rarityManager;
            _inventory      = inventory;
            _geometryOverlay = geometryOverlay;
            _trigOverlay     = trigOverlay;
            _difficulty     = difficulty;
        }

        /// <summary>
        /// Run a number of duel ticks.
        /// </summary>
        public void Tick(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var profile = OpponentProfiles.Challenge;
                var mood    = EnvironmentMood.Storm;
                BiasDescriptor? tilt = null;

                var opponent = _biasManager.GenerateOpponentWeighted(profile, mood, tilt);

                var runner = RunnerFactory.Create(
                    EngagementType.Duel,
                    _inventory,
                    _biasManager,
                    _facetManager,
                    _toneManager,
                    _rarityManager,
                    biasSeedId: null,
                    participants: new[] { _persistent.BiasVector, opponent }
                );

                runner.Run();

                var summary = runner.Engagement.Finalize();
                _tracker.AddSummary(summary);
                _tracker.AddResilience(summary.ResilienceIndex);

                // Geometry / trig metrics
                var duelPair = new List<(BiasVector, BiasVector)>
                {
                    (_persistent.BiasVector, opponent)
                };

                var hypotenuse = _geometryOverlay.ComputeHypotenuse(_persistent.BiasVector, opponent);
                var area       = _geometryOverlay.ComputeArea(_persistent.BiasVector, opponent);
                var cos        = _trigOverlay.MeanCos(duelPair);
                var sin        = _trigOverlay.MeanSin(duelPair);
                var log        = _trigOverlay.LogScaledIndex((int)Math.Round(summary.ResilienceIndex));
                var exp        = _trigOverlay.ExpScaledIndex((int)Math.Round(summary.ResilienceIndex));

                _tracker.AddHypotenuse(hypotenuse);
                _tracker.AddArea(area);
                _tracker.AddTrig(cos, sin, log, exp);

                // --- Forge crystals based on outcome ---
                var type = summary.Outcome switch
                {
                    DuelOutcome.Recovery    => CrystalType.Resilience,
                    DuelOutcome.Collapse    => CrystalType.Collapse,
                    DuelOutcome.Wound       => CrystalType.Wound,
                    DuelOutcome.Conflict    => CrystalType.Conflict,
                    DuelOutcome.Equilibrium => CrystalType.Equilibrium,
                    _                       => CrystalType.Resilience
                };

                // Aggregate facets and resolve tone cut
                var facets  = _facetManager.ResolveFacets(_persistent.BiasVector, opponent);
                var toneCut = _toneManager.ResolveToneCut(_persistent.BiasVector, opponent);

                // Assign rarity tier enriched with overlays
                var rarityTier = _rarityManager.AssignTier(
                    runner.Engagement,
                    hypotenuse,
                    area,
                    cos,
                    sin,
                    _tracker.AverageHypotenuse,
                    _tracker.CumulativeArea
                );

                // Factory call for the correct crystal type
                var crystal = TraitCrystalFactory.CreateCrystal(
                    threshold: (int)Math.Round(summary.ResilienceIndex),
                    isPositive: summary.Outcome == DuelOutcome.Recovery,
                    facets: facets,
                    narrative: summary.ToString(),
                    existingCrystals: _tracker.Crystals.ToList(),
                    toneCut: toneCut,
                    rarityTier: rarityTier,
                    type: type
                );

                // Narrative + tracker update
                var description = crystal.GetDescription(hypotenuse, area);
                _tracker.AddCrystal(crystal);
                _tracker.AddNarrative(description);

                _persistent.ApplyOutcome(summary, _difficulty);
            }
        }
    }
}
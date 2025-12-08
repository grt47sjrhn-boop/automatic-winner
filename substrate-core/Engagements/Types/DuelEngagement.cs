using System;
using System.Collections.Generic;
using substrate_core.Engagements.Base;
using substrate_core.Managers;
using substrate_core.Summaries.Types;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Mappers;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;

namespace substrate_core.Engagements.Types
{
    /// <summary>
    /// Engagement representing a duel between two bias vectors.
    /// </summary>
    public sealed class DuelEngagement : EngagementBase
    {
        private readonly InventoryManager _inventory;

        // 🔹 Injected managers
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public BiasVector DuelistA { get; }
        public BiasVector DuelistB { get; }
        public BiasVector ResolvedVector { get; private set; }
        public DuelOutcome Outcome { get; private set; }

        public DuelEngagement(
            InventoryManager inventory,
            BiasVector duelistA,
            BiasVector duelistB,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null)
        {
            _inventory     = inventory;
            DuelistA       = duelistA;
            DuelistB       = duelistB;
            _biasManager   = biasManager;
            _facetManager  = facetManager;
            _toneManager   = toneManager;
            _rarityManager = rarityManager;
            BiasSeedId     = biasSeedId;

            // Hydrate seed bias if provided
            if (BiasSeedId.HasValue)
            {
                var seedCrystal = _inventory.GetCrystal(BiasSeedId.Value);
                if (seedCrystal != null)
                {
                    Bias = new BiasDescriptor(
                        seedCrystal.GetBias(),
                        "Seeded from crystal facets",
                        seedCrystal.Facets
                    );
                }
            }
        }

        protected override void ResolveTick()
        {
            // Resolve vector using BiasManager
            ResolvedVector = _biasManager.Resolve(DuelistA, DuelistB, Bias);

            // Map resolved tone into a duel outcome
            switch (ResolvedVector.DominantTone)
            {
                case ToneType.Resilient:
                case ToneType.Radiant:
                case ToneType.Joy:
                case ToneType.Playful:
                    Outcome = DuelOutcome.Recovery;
                    break;

                case ToneType.Hostile:
                case ToneType.Critical:
                case ToneType.Despairing:
                case ToneType.Wound:
                    Outcome = DuelOutcome.Collapse;
                    break;

                case ToneType.Harmonious:
                case ToneType.Calm:
                case ToneType.Reflective:
                case ToneType.Equilibrium:
                    Outcome = DuelOutcome.Equilibrium;
                    break;

                case ToneType.Conflict:
                    Outcome = DuelOutcome.Conflict;
                    break;

                case ToneType.Composite:
                    Outcome = DuelOutcome.MixedConflict;
                    break;

                default:
                    Outcome = DuelOutcome.Unknown;
                    break;
            }

            // Update Shape from duel outcome (placeholder: normalize empty dictionary)
            Shape = _facetManager.Normalize(new Dictionary<FacetType,int>());

            // Update Bias
            Bias = _biasManager.Summarize(Shape);

            // Update Brilliance
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            Brilliance = _toneManager.Cut(toneDict);

            // Update Rarity
            var score = _rarityManager.ComputeScore(Shape);
            Rarity = _rarityManager.AssignTier(score);
        }

        public override void UpdateFacets()
        {
            Shape = _facetManager.Normalize(new Dictionary<FacetType,int>());
        }

        public override void UpdateBias()
        {
            Bias = _biasManager.Summarize(Shape);
        }

        public override void UpdateBrilliance()
        {
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            Brilliance = _toneManager.Cut(toneDict);
        }

        public override void UpdateRarity()
        {
            var score = _rarityManager.ComputeScore(Shape);
            Rarity = _rarityManager.AssignTier(score);
        }

        public override bool IsComplete =>
            Math.Abs(Shape.Values[FacetType.Resilience]) >= 6;

        public override ISummary Finalize()
        {
            return new DuelEventSummary(
                "Duel Engagement Complete",
                $"Resilience index: {Shape.Values[FacetType.Resilience]}",
                SummaryType.Duel,
                DuelistA,
                DuelistB,
                ResolvedVector,
                Outcome,
                Brilliance,
                Bias,
                isResolved: IsComplete
            );
        }
    }
}
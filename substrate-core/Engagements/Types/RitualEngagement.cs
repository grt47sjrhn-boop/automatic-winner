using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Engagements.Base;
using substrate_core.Managers;
using substrate_core.Summaries.Types;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Mappers;
using substrate_shared.Models;
using substrate_shared.structs;
using substrate_shared.types;

namespace substrate_core.Engagements.Types
{
    /// <summary>
    /// Engagement representing a ritual exchange among multiple participants.
    /// </summary>
    public sealed class RitualEngagement : EngagementBase
    {
        private readonly InventoryManager _inventory;

        // 🔹 Injected managers
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public IReadOnlyList<BiasVector> Participants { get; }
        public FacetDistribution CollectiveShape { get; private set; }
        public BiasDescriptor CollectiveBias { get; private set; }
        public ToneCut CollectiveBrilliance { get; private set; }
        public RarityTier RitualRarity { get; private set; }

        public RitualEngagement(
            InventoryManager inventory,
            IEnumerable<BiasVector> participants,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null)
        {
            _inventory     = inventory;
            Participants   = new List<BiasVector>(participants);
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
            // Ritual-specific tick logic:
            // - Aggregate participant vectors into a collective facet distribution
            var participantShapes = Participants.Select(p => p.ToFacetDistribution()).ToList();
            CollectiveShape = _facetManager.Aggregate(participantShapes);

            // If seeded bias exists, tilt the collective bias
            CollectiveBias = BiasSeedId.HasValue && Bias != null
                ? _biasManager.Combine(CollectiveShape, Bias)
                : _biasManager.Summarize(CollectiveShape);

            // Convert facets → tones → brilliance
            var toneDict = FacetToneMapper.ToToneDictionary(CollectiveShape);
            CollectiveBrilliance = _toneManager.Cut(toneDict);

            // Assign rarity tier
            var score = _rarityManager.ComputeScore(CollectiveShape);
            RitualRarity = _rarityManager.AssignTier(score);
        }

        public override void UpdateFacets()      => Shape = CollectiveShape;
        public override void UpdateBias()        => Bias = CollectiveBias;
        public override void UpdateBrilliance()  => Brilliance = CollectiveBrilliance;
        public override void UpdateRarity()      => Rarity = RitualRarity;

        // Example: ritual resolves after 3+ participants
        public override bool IsComplete => Participants.Count >= 3;

        public override ISummary Finalize()
        {
            return new RitualEventSummary(
                "Ritual Engagement Complete",
                $"Collective resonance achieved with {Participants.Count} participants",
                SummaryType.Ritual,
                Participants,
                CollectiveShape,
                CollectiveBias,
                CollectiveBrilliance,
                RitualRarity,
                isResolved: IsComplete
            );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Engagements.Base;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Mappers;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;

namespace substrate_shared.Engagements.Types
{
    /// <summary>
    /// Engagement representing a trial composed of multiple duels.
    /// </summary>
    public sealed class TrialEngagement : EngagementBase
    {
        private readonly InventoryManager _inventory;

        // 🔹 Injected managers
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public IReadOnlyList<DuelEngagement> Duels { get; }
        public BiasDescriptor AggregateBias { get; private set; }
        public ToneType DominantTone { get; private set; }
        public RarityTier TrialRarity { get; private set; }

        public TrialEngagement(
            InventoryManager inventory,
            IEnumerable<DuelEngagement> duels,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null)
        {
            _inventory     = inventory;
            Duels          = duels.ToList();
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
            // Advance each duel by one tick
            foreach (var duel in Duels)
            {
                duel.ResolveStep();
            }

            // Aggregate results across duels
            var allShapes = Duels.Select(d => d.Shape).ToList();
            Shape = _facetManager.Aggregate(allShapes);

            // Bias resolution
            AggregateBias = BiasSeedId.HasValue && Bias != null
                ? _biasManager.Combine(Shape, Bias)
                : _biasManager.Summarize(Shape);

            // Tone resolution
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            DominantTone = _toneManager.Cut(toneDict).Primary;

            // Rarity resolution
            var score = _rarityManager.ComputeScore(Shape);
            TrialRarity = _rarityManager.AssignTier(score);
        }

        public override void UpdateFacets() =>
            Shape = _facetManager.Aggregate(Duels.Select(d => d.Shape));

        public override void UpdateBias() =>
            Bias = AggregateBias;

        public override void UpdateBrilliance()
        {
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            Brilliance = _toneManager.Cut(toneDict);
        }

        public override void UpdateRarity() =>
            Rarity = TrialRarity;

        public override bool IsComplete =>
            Duels.All(d => d.IsComplete);

        public override ISummary Finalize()
        {
            return new TrialEventSummary(
                "Trial Engagement Complete",
                $"Trial resolved with {Duels.Count} duels, dominant tone {DominantTone}",
                SummaryType.Trial,
                Duels.Select(d => (DuelEventSummary)d.Finalize()),
                TrialRarity,
                AggregateBias,
                DominantTone,
                isResolved: IsComplete
            );
        }
    }
}
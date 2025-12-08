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
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;

namespace substrate_core.Engagements.Types
{
    /// <summary>
    /// Engagement representing a trial composed of multiple duels.
    /// </summary>
    public sealed class TrialEngagement : EngagementBase
    {
        private readonly InventoryManager _inventory;
        private readonly BiasManager _biasManager = new BiasManager();

        public IReadOnlyList<DuelEngagement> Duels { get; }
        public BiasDescriptor AggregateBias { get; private set; }
        public ToneType DominantTone { get; private set; }
        public RarityTier TrialRarity { get; private set; }

        public TrialEngagement(
            InventoryManager inventory,
            IEnumerable<DuelEngagement> duels,
            Guid? biasSeedId = null)
        {
            _inventory = inventory;
            Duels = duels.ToList();
            BiasSeedId = biasSeedId;

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
            Shape = FacetManager.Aggregate(allShapes);

            // Bias resolution
            AggregateBias = BiasSeedId.HasValue && Bias != null
                ? _biasManager.Combine(Shape, Bias)
                : _biasManager.Summarize(Shape);

            // Tone resolution
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            DominantTone = ToneManager.Cut(toneDict).Primary;

            // Rarity resolution
            TrialRarity = RarityManager.AssignTier(RarityManager.ComputeScore(this));
        }

        public override void UpdateFacets() =>
            Shape = FacetManager.Aggregate(Duels.Select(d => d.Shape));

        public override void UpdateBias() =>
            Bias = AggregateBias;

        public override void UpdateBrilliance()
        {
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            Brilliance = ToneManager.Cut(toneDict);
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
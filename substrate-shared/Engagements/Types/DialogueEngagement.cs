using System;
using System.Collections.Generic;
using substrate_shared.Engagements.Base;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Mappers;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;

namespace substrate_shared.Engagements.Types
{
    /// <summary>
    /// Engagement representing a dialogue exchange between two speakers.
    /// </summary>
    public sealed class DialogueEngagement : EngagementBase
    {
        private readonly InventoryManager _inventory;

        // 🔹 Injected managers
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public BiasVector SpeakerA { get; private set; }
        public BiasVector SpeakerB { get; private set; }
        public ToneType DominantTone { get; private set; }
        public bool IsBalanced { get; private set; }

        public DialogueEngagement(
            InventoryManager inventory,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null)
        {
            _inventory     = inventory;
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
            // Dialogue-specific tick logic:
            SpeakerA = BiasVector.GenerateRandom(); // TODO: replace with actual dialogue input
            SpeakerB = BiasVector.GenerateRandom();

            DominantTone = _toneManager.DetermineDominant(SpeakerA, SpeakerB);
            IsBalanced   = _toneManager.CheckBalance(SpeakerA, SpeakerB);
        }

        public override void UpdateFacets()
        {
            // Normalize dialogue outcomes into a facet distribution
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

        public override bool IsComplete => Shape.Values.Count >= 3;

        public override ISummary Finalize()
        {
            return new DialogueEventSummary(
                "Dialogue Engagement Complete",
                $"Dialogue resolved with dominant tone {DominantTone}",
                SummaryType.Dialogue,
                SpeakerA,
                SpeakerB,
                DominantTone,
                IsBalanced,
                isResolved: IsComplete
            );
        }
    }
}
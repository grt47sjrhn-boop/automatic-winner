using System;
using System.Collections.Generic;
using substrate_core.Engagements.Base;
using substrate_core.Managers;
using substrate_core.Summaries.Types;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Mappers;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.types;

namespace substrate_core.Engagements.Types
{
    /// <summary>
    /// Engagement representing a dialogue exchange between two speakers.
    /// </summary>
    public sealed class DialogueEngagement : EngagementBase
    {
        private readonly InventoryManager _inventory;
        private readonly BiasManager _biasManager = new BiasManager();

        public BiasVector SpeakerA { get; private set; }
        public BiasVector SpeakerB { get; private set; }
        public ToneType DominantTone { get; private set; }
        public bool IsBalanced { get; private set; }

        public DialogueEngagement(InventoryManager inventory, Guid? biasSeedId = null)
        {
            _inventory = inventory;
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
            // Dialogue-specific tick logic:
            SpeakerA = BiasVector.GenerateRandom(); // TODO: replace with actual dialogue input
            SpeakerB = BiasVector.GenerateRandom();

            DominantTone = ToneManager.DetermineDominant(SpeakerA, SpeakerB);
            IsBalanced   = ToneManager.CheckBalance(SpeakerA, SpeakerB);
        }

        public override void UpdateFacets()
        {
            // Normalize dialogue outcomes into a facet distribution
            Shape = FacetManager.Normalize(new Dictionary<FacetType,int>());
        }

        public override void UpdateBias()
        {
            // Use instance BiasManager
            Bias = _biasManager.Summarize(Shape);
        }

        public override void UpdateBrilliance()
        {
            // Convert facet distribution into tone distribution
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);

            // Brilliance now represented as ToneCut
            Brilliance = ToneManager.Cut(toneDict);
        }


        public override void UpdateRarity()
        {
            // Rarity now represented as RarityTier
            Rarity = RarityManager.AssignTier(RarityManager.ComputeScore(this));
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
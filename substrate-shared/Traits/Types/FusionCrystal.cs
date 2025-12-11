using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.Traits.Types
{
    public class FusionCrystal : TraitCrystal
    {
        public FusionCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            IToneCut toneCut,
            IRarityTier rarityTier)
            : base(CrystalType.Fusion, rarity, threshold, facets, toneCut, rarityTier) { }

        public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
        {
            // Hybrid effect: resilience and collapse tug simultaneously
            return outcome switch
            {
                DuelOutcome.Recovery => baseValue + (ModifierValue / 2),
                DuelOutcome.Wound or DuelOutcome.Conflict => baseValue - (ModifierValue / 2),
                _ => baseValue
            };
        }
    }
}
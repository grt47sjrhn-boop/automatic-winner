using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.Traits.Types
{
    public class WoundCrystal : TraitCrystal
    {
        public WoundCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            IToneCut toneCut,
            IRarityTier rarityTier)
            : base(CrystalType.Wound, rarity, threshold, facets, toneCut, rarityTier) { }

        public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
        {
            // Wound crystals exacerbate wounds, weaken recoveries
            return outcome switch
            {
                DuelOutcome.Wound    => baseValue - ModifierValue,
                DuelOutcome.Recovery => baseValue - (ModifierValue / 2),
                _ => baseValue
            };
        }
    }
}
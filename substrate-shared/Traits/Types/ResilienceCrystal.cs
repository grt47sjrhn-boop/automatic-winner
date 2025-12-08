using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;
using substrate_shared.Models;

namespace substrate_shared.Traits.Types
{
    public class ResilienceCrystal : TraitCrystal
    {
        public ResilienceCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            ToneCut toneCut,
            RarityTier rarityTier)
            : base(CrystalType.Resilience, rarity, threshold, facets, toneCut, rarityTier) { }

        public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
        {
            // Resilience boosts recovery and mitigates wounds
            return outcome switch
            {
                DuelOutcome.Recovery => baseValue + ModifierValue,
                DuelOutcome.Wound => baseValue - (ModifierValue / 2),
                _ => baseValue
            };
        }
    }
}
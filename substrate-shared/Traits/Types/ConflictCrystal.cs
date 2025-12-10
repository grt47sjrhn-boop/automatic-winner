using System.Collections.Generic;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Traits.Types
{
    public class ConflictCrystal : TraitCrystal
    {
        public ConflictCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            ToneCut toneCut,
            RarityTier rarityTier)
            : base(CrystalType.Conflict, rarity, threshold, facets, toneCut, rarityTier) { }

        public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
        {
            // Conflict crystals amplify tension, small oscillations
            return outcome switch
            {
                DuelOutcome.Conflict => baseValue + (ModifierValue / 2),
                _ => baseValue
            };
        }
    }
}
using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.Traits.Types
{
    public class ConflictCrystal : TraitCrystal
    {
        public ConflictCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            IToneCut toneCut,
            IRarityTier rarityTier)
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
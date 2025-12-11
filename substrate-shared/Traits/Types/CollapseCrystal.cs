using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.Traits.Types
{
    public class CollapseCrystal : TraitCrystal
    {
        public CollapseCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            IToneCut toneCut,
            IRarityTier rarityTier)
            : base(CrystalType.Collapse, rarity, threshold, facets, toneCut, rarityTier) { }

        public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
        {
            // Collapse amplifies wounds and conflicts
            return outcome switch
            {
                DuelOutcome.Wound => baseValue + ModifierValue,
                DuelOutcome.Conflict => baseValue + (ModifierValue / 2),
                _ => baseValue
            };
        }
    }
}
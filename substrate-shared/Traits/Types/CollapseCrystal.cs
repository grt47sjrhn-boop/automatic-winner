using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;
using substrate_shared.Models;

namespace substrate_shared.Traits.Types
{
    public class CollapseCrystal : TraitCrystal
    {
        public CollapseCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            ToneCut toneCut,
            RarityTier rarityTier)
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
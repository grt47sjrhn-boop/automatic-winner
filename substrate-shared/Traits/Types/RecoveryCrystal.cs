using System.Collections.Generic;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Traits.Types
{
    public class RecoveryCrystal : TraitCrystal
    {
        public RecoveryCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            ToneCut toneCut,
            RarityTier rarityTier)
            : base(CrystalType.Recovery, rarity, threshold, facets, toneCut, rarityTier) { }

        public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
        {
            // Recovery crystals strongly boost recoveries
            return outcome switch
            {
                DuelOutcome.Recovery => baseValue + (ModifierValue * 2),
                DuelOutcome.Collapse => baseValue - (ModifierValue / 2),
                _ => baseValue
            };
        }
    }
}
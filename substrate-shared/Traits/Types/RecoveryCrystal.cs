using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.Traits.Types
{
    public class RecoveryCrystal : TraitCrystal
    {
        public RecoveryCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            IToneCut toneCut,
            IRarityTier rarityTier)
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
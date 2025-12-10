using System.Collections.Generic;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Traits.Types
{
    public class EquilibriumCrystal : TraitCrystal
    {
        public EquilibriumCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            ToneCut toneCut,
            RarityTier rarityTier)
            : base(CrystalType.Equilibrium, rarity, threshold, facets, toneCut, rarityTier) { }

        public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
        {
            // Equilibrium crystals stabilize outcomes, dampening swings
            return outcome switch
            {
                DuelOutcome.Recovery   => baseValue + (ModifierValue / 4),
                DuelOutcome.Collapse   => baseValue - (ModifierValue / 4),
                DuelOutcome.Equilibrium => baseValue, // unchanged
                _ => baseValue
            };
        }
    }
}
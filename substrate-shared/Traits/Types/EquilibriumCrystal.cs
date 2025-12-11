using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.Traits.Types
{
    public class EquilibriumCrystal : TraitCrystal
    {
        public EquilibriumCrystal(
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity,
            IToneCut toneCut,
            IRarityTier rarityTier)
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
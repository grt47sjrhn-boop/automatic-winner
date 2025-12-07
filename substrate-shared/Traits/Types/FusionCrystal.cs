using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Traits.Types;

public class FusionCrystal : TraitCrystal
{
    public FusionCrystal(int threshold,
        IReadOnlyDictionary<ToneType,int> facets,
        CrystalRarity rarity = CrystalRarity.UltraRare)
        : base(CrystalType.Fusion, rarity, threshold, facets) { }

    public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
    {
        // Hybrid effect: resilience and collapse tug simultaneously
        return outcome switch
        {
            DuelOutcome.Recovery => baseValue + (ModifierValue / 2),
            DuelOutcome.Wound or DuelOutcome.Conflict => baseValue - (ModifierValue / 2),
            _ => baseValue
        };
    }
}
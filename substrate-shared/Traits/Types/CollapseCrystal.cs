using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Traits.Types;

public class CollapseCrystal : TraitCrystal
{
    public CollapseCrystal(int threshold,
        IReadOnlyDictionary<ToneType,int> facets,
        CrystalRarity rarity = CrystalRarity.Normal)
        : base(CrystalType.Collapse, rarity, threshold, facets) { }

    public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
    {
        return outcome switch
        {
            DuelOutcome.Recovery => baseValue - 1,
            DuelOutcome.Wound or DuelOutcome.Conflict => baseValue - ModifierValue,
            _ => baseValue
        };
    }
}
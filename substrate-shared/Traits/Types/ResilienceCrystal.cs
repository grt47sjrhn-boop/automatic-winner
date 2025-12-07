using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Traits.Types;

public class ResilienceCrystal : TraitCrystal
{
    public ResilienceCrystal(int threshold,
        IReadOnlyDictionary<ToneType,int> facets,
        CrystalRarity rarity = CrystalRarity.Normal)
        : base(CrystalType.Resilience, rarity, threshold, facets) { }

    public override int ModifyOutcome(DuelOutcome outcome, int baseValue)
    {
        return outcome switch
        {
            DuelOutcome.Recovery => baseValue + ModifierValue,
            DuelOutcome.Wound or DuelOutcome.Conflict => baseValue + 1,
            _ => baseValue
        };
    }
}
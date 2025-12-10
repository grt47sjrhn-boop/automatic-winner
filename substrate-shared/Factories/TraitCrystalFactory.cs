using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;
using substrate_shared.Traits.Types;
using substrate_shared.Models;

namespace substrate_shared.Factories
{
    public static class TraitCrystalFactory
    {
        private static readonly Random _rng = new Random();
        private static int resilienceCount = 0;
        private static int collapseCount = 0;
        private static bool rareResilienceExists = false;
        private static bool rareCollapseExists = false;

        // 🔹 Session reset for rare-roll state
        public static void ResetSession()
        {
            resilienceCount = 0;
            collapseCount   = 0;
            rareResilienceExists = false;
            rareCollapseExists   = false;
        }

        public static TraitCrystal CreateCrystal(
    int threshold,
    bool isPositive,
    IReadOnlyDictionary<ToneType,int> facets,
    string narrative,
    List<TraitCrystal> existingCrystals,
    ToneCut toneCut,
    RarityTier rarityTier,
    CrystalType type = CrystalType.Resilience // default branch
)
{
    // Ultra rare fusion roll
    if (existingCrystals.Any(c => c.Type == CrystalType.Resilience) &&
        existingCrystals.Any(c => c.Type == CrystalType.Collapse))
    {
        if (_rng.NextDouble() < 0.02) // 2% chance
        {
            return new FusionCrystal(
                threshold,
                facets,
                CrystalRarity.UltraRare,
                toneCut,
                rarityTier
            );
        }
    }

    switch (type)
    {
        case CrystalType.Resilience:
            resilienceCount++;
            var resChance = 0.1 * Math.Pow(2, resilienceCount - 1);
            if (resilienceCount > collapseCount) resChance *= 2;
            resChance = Math.Min(resChance, 0.5);
            var makeRareRes = !rareResilienceExists && _rng.NextDouble() < resChance;
            if (makeRareRes) rareResilienceExists = true;

            return new ResilienceCrystal(
                threshold,
                facets,
                makeRareRes ? CrystalRarity.Rare : CrystalRarity.Common,
                toneCut,
                rarityTier
            );

        case CrystalType.Collapse:
            collapseCount++;
            var colChance = 0.1 * Math.Pow(2, collapseCount - 1);
            if (collapseCount > resilienceCount) colChance *= 2;
            colChance = Math.Min(colChance, 0.5);
            var makeRareCol = !rareCollapseExists && _rng.NextDouble() < colChance;
            if (makeRareCol) rareCollapseExists = true;

            return new CollapseCrystal(
                threshold,
                facets,
                makeRareCol ? CrystalRarity.Rare : CrystalRarity.Common,
                toneCut,
                rarityTier
            );

        case CrystalType.Equilibrium:
            return new EquilibriumCrystal(
                threshold,
                facets,
                CrystalRarity.Equilibrium,
                toneCut,
                rarityTier
            );

        case CrystalType.Conflict:
            return new ConflictCrystal(
                threshold,
                facets,
                CrystalRarity.Common, // or Rare if you want tension salvage
                toneCut,
                rarityTier
            );

        case CrystalType.Wound:
            return new WoundCrystal(
                threshold,
                facets,
                CrystalRarity.Fragile,
                toneCut,
                rarityTier
            );

        case CrystalType.Recovery:
            return new RecoveryCrystal(
                threshold,
                facets,
                CrystalRarity.Rare,
                toneCut,
                rarityTier
            );

        case CrystalType.Fusion:
            return new FusionCrystal(
                threshold,
                facets,
                CrystalRarity.Rare,
                toneCut,
                rarityTier
            );
        default:
            // fallback to common resilience
            return new ResilienceCrystal(
                threshold,
                facets,
                CrystalRarity.Common,
                toneCut,
                rarityTier
            );
    }
}
    }
}
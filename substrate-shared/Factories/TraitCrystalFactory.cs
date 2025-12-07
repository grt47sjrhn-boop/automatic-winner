using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;
using substrate_shared.Traits.Types;

namespace substrate_shared.Factories
{
    public static class TraitCrystalFactory
    {
        private static readonly Random _rng = new Random();
        private static int resilienceCount = 0;
        private static int collapseCount = 0;
        private static bool rareResilienceExists = false;
        private static bool rareCollapseExists = false;

        public static TraitCrystal CreateCrystal(
            int threshold,
            bool isPositive,
            IReadOnlyDictionary<ToneType,int> facets,
            string narrative,
            List<TraitCrystal> existingCrystals)
        {
            // 🔹 Ultra rare fusion roll
            if (existingCrystals.Any(c => c.Type == CrystalType.Resilience) &&
                existingCrystals.Any(c => c.Type == CrystalType.Collapse))
            {
                if (_rng.NextDouble() < 0.02) // 2% chance
                {
                    return new FusionCrystal(threshold, facets, CrystalRarity.UltraRare);
                }
            }

            // 🔹 Rare chance logic
            bool makeRare = false;
            if (isPositive)
            {
                resilienceCount++;
                double chance = 0.1 * Math.Pow(2, resilienceCount - 1);
                if (resilienceCount > collapseCount) chance *= 2;
                chance = Math.Min(chance, 0.5); // cap at 50%
                makeRare = !rareResilienceExists && _rng.NextDouble() < chance;
                if (makeRare) rareResilienceExists = true;

                return new ResilienceCrystal(threshold, facets,
                    makeRare ? CrystalRarity.Rare : CrystalRarity.Normal);
            }
            else
            {
                collapseCount++;
                double chance = 0.1 * Math.Pow(2, collapseCount - 1);
                if (collapseCount > resilienceCount) chance *= 2;
                chance = Math.Min(chance, 0.5);
                makeRare = !rareCollapseExists && _rng.NextDouble() < chance;
                if (makeRare) rareCollapseExists = true;

                return new CollapseCrystal(threshold, facets,
                    makeRare ? CrystalRarity.Rare : CrystalRarity.Normal);
            }
        }
    }
}
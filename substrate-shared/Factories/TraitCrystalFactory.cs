using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;
using substrate_shared.Traits.Types;
using substrate_core.Engagements.Results;
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

        public static TraitCrystal CreateCrystal(
            int threshold,
            bool isPositive,
            IReadOnlyDictionary<ToneType,int> facets,
            string narrative,
            List<TraitCrystal> existingCrystals,
            ToneCut toneCut,
            RarityTier rarityTier)
        {
            // 🔹 Ultra rare fusion roll
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

            // 🔹 Rare chance logic
            var makeRare = false;
            if (isPositive)
            {
                resilienceCount++;
                var chance = 0.1 * Math.Pow(2, resilienceCount - 1);
                if (resilienceCount > collapseCount) chance *= 2;
                chance = Math.Min(chance, 0.5); // cap at 50%
                makeRare = !rareResilienceExists && _rng.NextDouble() < chance;
                if (makeRare) rareResilienceExists = true;

                return new ResilienceCrystal(
                    threshold,
                    facets,
                    makeRare ? CrystalRarity.Rare : CrystalRarity.Common,
                    toneCut,
                    rarityTier
                );
            }
            else
            {
                collapseCount++;
                var chance = 0.1 * Math.Pow(2, collapseCount - 1);
                if (collapseCount > resilienceCount) chance *= 2;
                chance = Math.Min(chance, 0.5);
                makeRare = !rareCollapseExists && _rng.NextDouble() < chance;
                if (makeRare) rareCollapseExists = true;

                return new CollapseCrystal(
                    threshold,
                    facets,
                    makeRare ? CrystalRarity.Rare : CrystalRarity.Common,
                    toneCut,
                    rarityTier
                );
            }
        }
    }
}
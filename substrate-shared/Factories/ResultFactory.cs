using System.Collections.Generic;
using System.Linq;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_core.Engagements.Results
{
    public static class ResultFactory
    {
        public static ToneCut CreateToneCut(IReadOnlyDictionary<ToneType,int> facets, int threshold = 1)
        {
            var cut = new ToneCut();
            int max = int.MinValue;

            foreach (var kv in facets)
            {
                if (kv.Value > max)
                {
                    cut.Primary = kv.Key;
                    max = kv.Value;
                }

                if (kv.Value >= threshold)
                    cut.Distribution[kv.Key] = kv.Value;
            }

            return cut;
        }

        /// <summary>
        /// Create a rarity tier directly from a TraitCrystal.
        /// </summary>
        public static RarityTier CreateRarityTier(TraitCrystal crystal)
        {
            return CreateRarityTier(crystal.Threshold, crystal.Facets);
        }

        /// <summary>
        /// Create a rarity tier from threshold + facets (used during forging).
        /// </summary>
        public static RarityTier CreateRarityTier(int threshold, IReadOnlyDictionary<ToneType,int> facets)
        {
            var tier = new RarityTier();
            var sum = facets.Values.Sum();

            if (sum < 10)
            {
                tier.Tier = "Common";
                tier.Description = "Basic resonance, easily forged.";
            }
            else if (sum < 20)
            {
                tier.Tier = "Rare";
                tier.Description = "Distinct resonance, moderately uncommon.";
            }
            else if (sum < 30)
            {
                tier.Tier = "Epic";
                tier.Description = "Powerful resonance, rare to encounter.";
            }
            else if (sum < 50)
            {
                tier.Tier = "Mythic";
                tier.Description = "Legendary resonance, forged in extraordinary conditions.";
            }
            else
            {
                tier.Tier = "UltraRare";
                tier.Description = "Transcendent resonance, bending fate itself.";
            }

            return tier;
        }
    }
}
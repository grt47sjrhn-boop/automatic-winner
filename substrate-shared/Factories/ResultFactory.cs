using System.Collections.Generic;
using System.Linq;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.Factories
{
    public static class ResultFactory
    {
        public static ToneCut CreateToneCut(IReadOnlyDictionary<ToneType,int> facets, int threshold = 1)
        {
            var cut = new ToneCut();
            var max = int.MinValue;

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
        /// Recovery-only path (legacy).
        /// </summary>
        public static RarityTier CreateRarityTier(int threshold, IReadOnlyDictionary<ToneType,int> facets)
        {
            var sum = facets.Values.Sum();
            var tier = new RarityTier();

            if (sum < 10)
            {
                tier.Tier = "Common";
                tier.Description = "Common crystal: faint glimmers forged at low thresholds.";
            }
            else if (sum < 20)
            {
                tier.Tier = "Rare";
                tier.Description = "Rare crystal: resilience forged at moderate thresholds.";
            }
            else if (sum < 30)
            {
                tier.Tier = "Epic";
                tier.Description = "Epic crystal: radiant arcs forged in duels.";
            }
            else if (sum < 50)
            {
                tier.Tier = "Mythic";
                tier.Description = "Mythic crystal: extraordinary resonance forged in rare conditions.";
            }
            else
            {
                tier.Tier = "UltraRare";
                tier.Description = "UltraRare crystal: transcendent resonance bending fate itself.";
            }

            return tier;
        }

        /// <summary>
        /// Outcome-aware path: collapse duels produce Fragile/Corrupted/Doomed.
        /// </summary>
        public static RarityTier CreateRarityTier(DuelOutcome outcome, int threshold, IReadOnlyDictionary<ToneType,int> facets)
        {
            var sum = facets.Values.Sum();
            var tier = new RarityTier();

            if (outcome == DuelOutcome.Collapse)
            {
                var dominantNegative = facets
                    .Where(kv => kv.Key == ToneType.Hostile
                              || kv.Key == ToneType.Critical
                              || kv.Key == ToneType.Despairing
                              || kv.Key == ToneType.Wound
                              || kv.Key == ToneType.Forsaken
                              || kv.Key == ToneType.Corrupted
                              || kv.Key == ToneType.Doomed)
                    .Sum(kv => kv.Value);

                if (dominantNegative < 10)
                {
                    tier.Tier = "Fragile";
                    tier.Description = "Fragile crystal: brittle shard born of collapse.";
                }
                else if (dominantNegative < 20)
                {
                    tier.Tier = "Corrupted";
                    tier.Description = "Corrupted crystal: resonance twisted into despair.";
                }
                else
                {
                    tier.Tier = "Doomed";
                    tier.Description = "Doomed crystal: artifact of irreversible collapse.";
                }
            }
            else
            {
                // fallback to recovery path
                return CreateRarityTier(threshold, facets);
            }

            return tier;
        }
    }
}
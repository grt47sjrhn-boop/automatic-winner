using System;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.enums.Attributes;

namespace substrate_core.Managers
{
    /// <summary>
    /// Manager responsible for computing, assigning, and narrating rarity tiers.
    /// Implements IManager for orchestration consistency.
    /// </summary>
    public class RarityManager : IManager
    {
        /// <summary>
        /// Compute a rarity score from engagement facets.
        /// Example: sum resilience + harmony - conflict.
        /// </summary>
        public static int ComputeScore(IEngagement engagement)
        {
            var shape = engagement.Shape;
            int resilience = shape.Values[FacetType.Resilience];
            int harmony    = shape.Values[FacetType.Harmony];
            int conflict   = shape.Values[FacetType.Conflict];

            return resilience + harmony - conflict;
        }

        /// <summary>
        /// Assign a descriptive rarity tier based on score thresholds.
        /// Wraps the CrystalRarity enum with a RarityTier model.
        /// </summary>
        public static RarityTier AssignTier(int score)
        {
            CrystalRarity rarity = score switch
            {
                < 3  => CrystalRarity.Common,
                < 6  => CrystalRarity.Rare,
                < 10 => CrystalRarity.Epic,
                _    => CrystalRarity.Legendary
            };

            string description = GetNarrative(rarity);
            return new RarityTier(rarity.ToString(), description);
        }

        /// <summary>
        /// Get narrative notes from the CrystalRarity enum attributes.
        /// </summary>
        public static string GetNarrative(CrystalRarity rarity)
        {
            var field = typeof(CrystalRarity).GetField(rarity.ToString());
            var attr = (RegistryNarrativeAttribute?)Attribute.GetCustomAttribute(field!, typeof(RegistryNarrativeAttribute));

            return attr?.Description ?? rarity.ToString();
        }

        /// <summary>
        /// Full rarity report: tier + narrative.
        /// </summary>
        public static string DescribeRarity(CrystalRarity rarity)
        {
            var narrative = GetNarrative(rarity);
            return $"{rarity}: {narrative}";
        }
    }
}
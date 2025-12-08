using System;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.enums.Attributes;
using substrate_shared.structs;

namespace substrate_core.Managers
{
    /// <summary>
    /// Manager responsible for computing, assigning, and narrating rarity tiers.
    /// Implements IRarityManager for orchestration consistency.
    /// </summary>
    public class RarityManager : IRarityManager
    {
        /// <summary>
        /// Compute a rarity score directly from a facet distribution.
        /// Example: sum resilience + harmony - conflict.
        /// </summary>
        public int ComputeScore(FacetDistribution distribution)
        {
            var resilience = distribution.Values.ContainsKey(FacetType.Resilience)
                ? distribution.Values[FacetType.Resilience] : 0;
            var harmony    = distribution.Values.ContainsKey(FacetType.Harmony)
                ? distribution.Values[FacetType.Harmony] : 0;
            var conflict   = distribution.Values.ContainsKey(FacetType.Conflict)
                ? distribution.Values[FacetType.Conflict] : 0;

            return resilience + harmony - conflict;
        }

        /// <summary>
        /// Compute a rarity score from an engagement (delegates to distribution-based scoring).
        /// </summary>
        public int ComputeScore(IEngagement engagement)
        {
            return ComputeScore(engagement.Shape);
        }

        /// <summary>
        /// Assign a descriptive rarity tier based on score thresholds.
        /// Wraps the CrystalRarity enum with a RarityTier model.
        /// </summary>
        public RarityTier AssignTier(int score)
        {
            var rarity = score switch
            {
                < 3  => CrystalRarity.Common,
                < 6  => CrystalRarity.Rare,
                < 10 => CrystalRarity.Epic,
                _    => CrystalRarity.Legendary
            };

            var description = GetNarrative(rarity);
            return new RarityTier(rarity.ToString(), description);
        }

        /// <summary>
        /// Get narrative notes from the CrystalRarity enum attributes.
        /// </summary>
        private static string GetNarrative(CrystalRarity rarity)
        {
            var field = typeof(CrystalRarity).GetField(rarity.ToString());
            var attr = (RegistryNarrativeAttribute?)Attribute.GetCustomAttribute(field!, typeof(RegistryNarrativeAttribute));

            return attr?.Description ?? rarity.ToString();
        }

        /// <summary>
        /// Full rarity report: tier + narrative.
        /// </summary>
        public string DescribeRarity(CrystalRarity rarity)
        {
            var narrative = GetNarrative(rarity);
            return $"{rarity}: {narrative}";
        }
    }
}
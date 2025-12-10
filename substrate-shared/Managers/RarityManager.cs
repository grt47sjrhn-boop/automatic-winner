using System;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.enums.Attributes;
using substrate_shared.structs;

namespace substrate_shared.Managers
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
        /// Compute a rarity score from distribution + geometry overlays, normalized against averages.
        /// </summary>
        public int ComputeScore(
            FacetDistribution distribution,
            double hypotenuse,
            double area,
            double cos,
            double sin,
            double avgHypotenuse,
            double avgArea)
        {
            var baseScore = ComputeScore(distribution);

            // Normalize hypotenuse relative to average
            var hypFactor = avgHypotenuse > 0 ? hypotenuse / avgHypotenuse : 1.0;
            if (hypFactor > 1.5) baseScore += 2;        // wide battles
            else if (hypFactor < 0.5) baseScore -= 1;   // cramped collapses

            // Normalize area relative to average
            var areaFactor = avgArea > 0 ? area / avgArea : 1.0;
            if (areaFactor > 2.0) baseScore += 3;       // sprawling clashes
            else if (areaFactor < 0.5) baseScore -= 1;  // tight footprint

            // Angle influence
            if (cos < 0.5) baseScore += 1;              // off‑angle runs
            if (Math.Abs(sin) > 0.7) baseScore += 1;    // lateral drift

            return baseScore;
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
        /// Assign a rarity tier using engagement + geometry overlays.
        /// </summary>
        public RarityTier AssignTier(
            IEngagement engagement,
            double hypotenuse,
            double area,
            double cos,
            double sin,
            double avgHypotenuse,
            double avgArea)
        {
            var score = ComputeScore(engagement.Shape, hypotenuse, area, cos, sin, avgHypotenuse, avgArea);
            return AssignTier(score);
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
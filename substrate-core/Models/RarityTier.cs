using substrate_shared.interfaces;
using substrate_shared.interfaces.Details;

namespace substrate_core.Models
{
    /// <summary>
    /// Represents the rarity classification of a crystal.
    /// </summary>
    public class RarityTier : IRarityTier
    {
        /// <summary>
        /// The rarity tier label (Common, Rare, Epic, Mythic, UltraRare).
        /// </summary>
        public string Tier { get; set; } = "Common";

        /// <summary>
        /// Narrative description of the tier.
        /// </summary>
        public string Description { get; set; } = "Default rarity tier.";

        public RarityTier() { }

        public RarityTier(string tier, string description)
        {
            Tier = tier;
            Description = description;
        }

        public override string ToString() => $"{Tier} → {Description}";
    }
}
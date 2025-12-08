namespace substrate_shared.Models
{
    /// <summary>
    /// Represents the rarity classification of a crystal.
    /// </summary>
    public class RarityTier
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
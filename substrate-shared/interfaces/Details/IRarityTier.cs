namespace substrate_shared.interfaces.Details
{
    /// <summary>
    /// Contract for rarity classification of a crystal.
    /// Provides tier label and narrative description.
    /// </summary>
    public interface IRarityTier
    {
        /// <summary>
        /// The rarity tier label (Common, Rare, Epic, Mythic, UltraRare).
        /// </summary>
        string Tier { get; set; }

        /// <summary>
        /// Narrative description of the tier.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// String representation of the rarity tier.
        /// </summary>
        string ToString();
    }
}
using substrate_shared.enums;
using substrate_shared.structs;

namespace substrate_shared.types.core
{
    /// <summary>
    /// Formats cycle output into narratable strings for lore logging or UI display.
    /// </summary>
    public sealed class LoreFormatter
    {
        public string FormatCycle(MoodVector input, BiasDelta delta, float magnitude,
            ResolutionLayers layers, ArtifactCore artifact, Trait trait)
        {
            return $"Cycle resonates with {DescribeArtifact(artifact.Type)}.\n" +
                   $"Trait unlocked: {trait.Name} (Tier {trait.Tier}).\n" +
                   $"Magnitude {magnitude:F2}, tones → D:{layers.Dominant:F2}, S:{layers.Secondary:F2}, H:{layers.Hidden:F2}.";
        }

        private string DescribeArtifact(ArtifactType type)
        {
            return type switch
            {
                ArtifactType.Pearl => "a common pearl of clarity",
                ArtifactType.RarePearl => "a rare pearl of resilience",
                ArtifactType.ChaosCrystal => "a chaos crystal of fracture",
                ArtifactType.SilentCycle => "a silent cycle of stillness",
                ArtifactType.CatastrophicCollapse => "a catastrophic collapse, shardstorm unleashed",
                _ => "an unknown resonance"
            };
        }

        #region Summary
        // Signature: LoreFormatter.FormatCycle(MoodVector input, BiasDelta delta, float magnitude, ResolutionLayers layers, ArtifactCore artifact, Trait trait) -> string
        // PRISMx Version: 1.1 | Date: 2025.111.28
        // Tags: #lore #formatter #substrate
        #endregion
    }
}
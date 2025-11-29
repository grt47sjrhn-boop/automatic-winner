using System;
using substrate_shared.interfaces.core;
using substrate_shared.structs;
using substrate_shared.types.core;

namespace substrate_shared.Defaults
{
    /// <summary>
    /// Logs cycle outcomes to console. Can be swapped for Unity UI or file logging.
    /// </summary>
    public sealed class DefaultLoreLogger : ILoreLogger
    {
        public void LogCycle(in MoodVector input, in BiasDelta delta, float magnitude,
            in ResolutionLayers layers, in ArtifactCore artifact, in Trait trait)
        {
            Console.WriteLine($"Cycle Log:");
            Console.WriteLine($"  Input: {input}");
            Console.WriteLine($"  Δ: {delta}, Magnitude: {magnitude:F2}");
            Console.WriteLine($"  Resolution → Dominant: {layers.Dominant:F2}, Secondary: {layers.Secondary:F2}, Hidden: {layers.Hidden:F2}");
            Console.WriteLine($"  Artifact: {artifact.Type}, Trait: {trait.Name} (Tier {trait.Tier})");
        }

        #region Summary
        // Signature: DefaultLoreLogger.LogCycle(MoodVector input, BiasDelta delta, float magnitude, ResolutionLayers layers, ArtifactCore artifact, Trait trait)
        // PRISMx Version: 1.1 | Date: 2025.111.28
        // Tags: #substrate #dll #unity
        #endregion
    }
}
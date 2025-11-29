using System;
using substrate_shared.enums;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.extensions;
using substrate_shared.structs;
using substrate_shared.types.core;

namespace substrate_extensions
{
    /// <summary>
    /// Default contributor sandbox. Responds to artifact resolution and bias drift events.
    /// Can be subclassed or replaced by contributors to inject custom logic.
    /// </summary>
    public sealed class ContributorSandbox : IArtifactHook, IBiasDriftHook, ILoreLogger
    {
        public void OnArtifactResolved(Artifact artifact, BiasVector bias)
        {
            switch (artifact.Type)
            {
                case ArtifactType.Pearl:
                    Console.WriteLine($"[Sandbox] Pearl resolved → Trait: {artifact.Trait.Name} (Tier {artifact.Trait.Tier})");
                    break;

                case ArtifactType.RarePearl:
                    Console.WriteLine($"[Sandbox] Rare Pearl! Trait: {artifact.Trait.Name} (Tier {artifact.Trait.Tier}) → Boost economy.");
                    break;

                case ArtifactType.ChaosCrystal:
                    Console.WriteLine($"[Sandbox] Chaos Crystal! Trait: {artifact.Trait.Name} → Fracture atmosphere.");
                    break;

                case ArtifactType.CatastrophicCollapse:
                    Console.WriteLine($"[Sandbox] Collapse! Trait: {artifact.Trait.Name} → Trigger shardstorm.");
                    break;

                case ArtifactType.SilentCycle:
                    Console.WriteLine($"[Sandbox] Stillness cycle. No trait resolved.");
                    break;
            }
        }

        public void OnBiasDrift(BiasVector oldBias, BiasVector newBias)
        {
            Console.WriteLine($"[Sandbox] Bias drifted:");
            Console.WriteLine($"  Δ Resonance: {newBias.Resonance - oldBias.Resonance:F2}");
            Console.WriteLine($"  Δ Warmth: {newBias.Warmth - oldBias.Warmth:F2}");
            Console.WriteLine($"  Δ Irony: {newBias.Irony - oldBias.Irony:F2}");
            Console.WriteLine($"  Δ Flux: {newBias.Flux - oldBias.Flux:F2}");
            Console.WriteLine($"  Δ Inertia: {newBias.Inertia - oldBias.Inertia:F2}");
            Console.WriteLine($"  Δ Variance: {newBias.Variance - oldBias.Variance:F2}");
            Console.WriteLine($"  Δ HiddenPressure: {newBias.HiddenPressure - oldBias.HiddenPressure:F2}");
        }

        public void LogCycle(in MoodVector input, in BiasDelta delta, float magnitude,
                             in ResolutionLayers layers, in ArtifactCore artifact, in Trait trait)
        {
            Console.WriteLine($"[Sandbox Lore] Cycle Log:");
            Console.WriteLine($"  Input → {input}");
            Console.WriteLine($"  Δ → {delta}, Magnitude: {magnitude:F2}");
            Console.WriteLine($"  Layers → D:{layers.Dominant:F2} S:{layers.Secondary:F2} H:{layers.Hidden:F2}");
            Console.WriteLine($"  Artifact → {artifact.Type}, Trait → {trait.Name} (Tier {trait.Tier})");
        }

        #region Summary
        // Signature: ContributorSandbox.OnArtifactResolved(Artifact artifact, BiasVector bias) -> void
        // PRISMx Version: 1.1 | Date: 2025.111.28
        // Tags: #extensions #hooks #sandbox
        #endregion
    }
}
using System;
using System.Runtime.CompilerServices;
using substrate_shared.enums;
using substrate_shared.interfaces.core;
using substrate_shared.structs;
using substrate_shared.types.core;
using ArtifactCore = substrate_shared.structs.ArtifactCore;
using ResolutionLayers = substrate_shared.structs.ResolutionLayers;

namespace substrate_shared.Defaults.Policies
{
    /// <summary>
    /// Bias drift policy with artifact-aware alpha scaling, dominant-axis emphasis,
    /// and robust clamping. Deterministic and side-effect free.
    /// </summary>
    public sealed class DefaultBiasDriftPolicy : IBiasDriftPolicy
    {
        private readonly DriftParameters _params;

        // Smaller epsilon so tiny-but-nonzero magnitudes can still drift in tests
        private const float MagnitudeEpsilon = 1e-9f;

        public DefaultBiasDriftPolicy(DriftParameters parameters)
        {
            _params = parameters;
        }

        public BiasVector Apply(
            in BiasVector current,
            in BiasDelta delta,
            float magnitude,
            in ArtifactCore artifact,
            in ResolutionLayers layers)
        {
            // Edge cases first
            if (IsSilentCycle(magnitude, artifact))
                return current; // no drift

            // Base alpha by artifact type (pre-boost)
            float alphaBase = artifact.Type switch
            {
                ArtifactType.CatastrophicCollapse => _params.CollapseBase + magnitude * _params.CollapseSlope,
                ArtifactType.RarePearl            => _params.RareBase     + magnitude * _params.RareSlope,
                ArtifactType.ChaosCrystal         => _params.ChaosBase    + magnitude * _params.ChaosSlope,
                ArtifactType.Pearl                => _params.CommonBase   + magnitude * _params.CommonSlope,
                _                                 => _params.CommonBase   + magnitude * _params.CommonSlope
            };

            // Clamp base alpha to keep within [0,1]
            float alpha = Clamp01(alphaBase);

            // Dominant emphasis: amplify alpha based on dominant layer
            // Use unclamped dominant contribution (can be negative/positive),
            // but clamp the final alpha post-boost to [0,1].
            float boost = 1f + (layers.Dominant * _params.DominantEmphasis);
            alpha = Clamp01(alpha * boost);

            Console.WriteLine($"Alpha: {alpha}");
            
            // If alpha is effectively zero, short-circuit to avoid pointless work
            if (alpha <= 0f)
                return current;

            return current.AddScaled(delta, alpha);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSilentCycle(float magnitude, in ArtifactCore artifact)
        {
            // Explicit SilentCycle or near-zero magnitude => no drift
            return artifact.Type == ArtifactType.SilentCycle || magnitude <= MagnitudeEpsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Clamp01(float v)
        {
            // Fast branchless clamp
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }

    #region Summary
    // Signature: DefaultBiasDriftPolicy.Apply(BiasVector current, BiasDelta delta, float magnitude, ArtifactCore artifact, ResolutionLayers layers) -> BiasVector
    // Notes:
    // - Post-boost clamp keeps alpha bounded even after dominant emphasis.
    // - Smaller epsilon allows visible drift for tiny-but-nonzero magnitudes in tests.
    // - Deterministic, pure function suitable for concurrent use.
    // PRISMx Version: 1.2 | Date: 2025.11.29
    // Tags: #substrate #drift #policy
    #endregion
}
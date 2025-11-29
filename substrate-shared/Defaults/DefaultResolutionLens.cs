using System;
using substrate_shared.interfaces.core;
using substrate_shared.structs;
using substrate_shared.types.core;

namespace substrate_shared.Defaults
{
    /// <summary>
    /// Default resolution lens implementing triangle schema:
    /// - Dominant: strongest axis in delta
    /// - Secondary: next strongest
    /// - Hidden: orthogonal undertone (sine component)
    /// </summary>
    public sealed class DefaultResolutionLens : IResolutionLens
    {
        public ResolutionLayers Resolve(in BiasDelta delta, in BiasVector bias, float magnitude)
        {
            // Simplified: pick dominant/secondary by absolute value
            float[] values = {
                delta.Resonance, delta.Warmth, delta.Irony,
                delta.Flux, delta.Inertia, delta.Variance, delta.HiddenPressure
            };

            // Find dominant and secondary
            int dominantIndex = 0, secondaryIndex = 1;
            for (int i = 0; i < values.Length; i++)
            {
                if (Math.Abs(values[i]) > Math.Abs(values[dominantIndex]))
                {
                    secondaryIndex = dominantIndex;
                    dominantIndex = i;
                }
                else if (Math.Abs(values[i]) > Math.Abs(values[secondaryIndex]))
                {
                    secondaryIndex = i;
                }
            }

            float dominant = values[dominantIndex];
            float secondary = values[secondaryIndex];
            float hidden = delta.HiddenPressure;

            return new ResolutionLayers(dominant, secondary, hidden);
        }

        #region Summary
        // Signature: DefaultResolutionLens.Resolve(BiasDelta delta, BiasVector bias, float magnitude) -> ResolutionLayers
        // PRISMx Version: 1.1 | Date: 2025.111.28
        // Tags: #substrate #dll #unity
        #endregion
    }
}
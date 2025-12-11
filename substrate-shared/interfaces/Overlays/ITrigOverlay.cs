using System.Collections.Generic;
using substrate_shared.structs;

namespace substrate_shared.interfaces.Overlays
{
    /// <summary>
    /// Contract for trigonometric overlay calculations.
    /// Provides aggregate metrics like average hypotenuse, cumulative area, mean cos/sin, and scaled indices.
    /// </summary>
    public interface ITrigOverlay : IOverlay
    {
        double AverageHypotenuse(IEnumerable<(BiasVector, BiasVector)> duels);
        double CumulativeArea(IEnumerable<(BiasVector, BiasVector)> duels);
        double MeanCos(IEnumerable<(BiasVector, BiasVector)> duels);
        double MeanSin(IEnumerable<(BiasVector, BiasVector)> duels);
        double LogScaledIndex(int resilienceIndex);
        double ExpScaledIndex(int resilienceIndex);
    }
}
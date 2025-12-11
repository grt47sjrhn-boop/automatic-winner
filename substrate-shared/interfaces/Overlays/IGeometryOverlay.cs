using substrate_shared.structs;

namespace substrate_shared.interfaces.Overlays
{
    /// <summary>
    /// Contract for geometric overlay calculations.
    /// Provides narratable metrics like hypotenuse, area, and descriptive overlays.
    /// </summary>
    public interface IGeometryOverlay : IOverlay
    {
        /// <summary>
        /// Compute the hypotenuse from two bias vectors.
        /// </summary>
        double ComputeHypotenuse(BiasVector a, BiasVector b);

        /// <summary>
        /// Compute the area formed by two bias vectors.
        /// </summary>
        double ComputeArea(BiasVector a, BiasVector b);

        /// <summary>
        /// Describe the overlay with hypotenuse and area values.
        /// </summary>
        string DescribeOverlay(BiasVector a, BiasVector b);
    }
}
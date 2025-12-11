using System;
using substrate_shared.interfaces.Overlays;
using substrate_shared.structs;

namespace substrate_core.Overlays
{
    /// <summary>
    /// Manager responsible for geometric overlay calculations.
    /// Converts bias vectors into narratable metrics like hypotenuse and area.
    /// </summary>
    public class GeometryOverlay : IGeometryOverlay
    {
        public double ComputeHypotenuse(BiasVector a, BiasVector b)
        {
            var p = a.SignedStrength > 0 ? a.Magnitude : 0;
            var n = b.SignedStrength < 0 ? b.Magnitude : 0;
            return Math.Sqrt(Math.Pow(p, 2) + Math.Pow(n, 2));
        }

        public double ComputeArea(BiasVector a, BiasVector b)
        {
            var magA = Math.Max(1.0, a.Magnitude);
            var magB = Math.Max(1.0, b.Magnitude);

            var angle = AngleBetween(a, b);
            return magA * magB * Math.Sin(angle);
        }

        public string DescribeOverlay(BiasVector a, BiasVector b)
        {
            var hypotenuse = ComputeHypotenuse(a, b);
            var area       = ComputeArea(a, b);
            return $"Overlay → Hypotenuse: {hypotenuse:F2}, Area: {area:F2}";
        }

        // 🔹 Private helper
        private double AngleBetween(BiasVector a, BiasVector b)
        {
            var sa = a.SignedStrength;
            var sb = b.SignedStrength;

            var denom = Math.Max(1e-6, Math.Abs(sa) * Math.Abs(sb));
            var cos   = denom > 0 ? (sa * sb) / denom : 0;

            // Clamp to [-1,1] to avoid domain errors
            cos = Math.Clamp(cos, -1.0, 1.0);

            return Math.Acos(cos);
        }
    }
}
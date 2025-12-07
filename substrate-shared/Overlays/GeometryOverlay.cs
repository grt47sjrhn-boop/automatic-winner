using System;
using substrate_shared.structs;

namespace substrate_shared.Overlays
{
    public static class GeometryOverlay
    {
        public static double ComputeHypotenuse(BiasVector a, BiasVector b)
        {
            var p = a.SignedStrength > 0 ? a.Magnitude : 0;
            var n = b.SignedStrength < 0 ? b.Magnitude : 0;
            return Math.Sqrt(Math.Pow(p, 2) + Math.Pow(n, 2));
        }

        public static double ComputeArea(BiasVector a, BiasVector b)
        {
            var p = a.SignedStrength > 0 ? a.Magnitude : 0;
            var n = b.SignedStrength < 0 ? b.Magnitude : 0;
            return 0.5 * p * n;
        }

        public static string DescribeOverlay(BiasVector a, BiasVector b)
        {
            var hypotenuse = ComputeHypotenuse(a, b);
            var area = ComputeArea(a, b);
            return $"Overlay → Hypotenuse: {hypotenuse:F2}, Area: {area:F2}";
        }
    }
}
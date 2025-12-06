using System;
using substrate_shared.structs;

namespace substrate_shared.Overlays
{
    public static class GeometryOverlay
    {
        public static double ComputeHypotenuse(BiasVector a, BiasVector b)
        {
            int p = a.SignedStrength > 0 ? a.Magnitude : 0;
            int n = b.SignedStrength < 0 ? b.Magnitude : 0;
            return Math.Sqrt(Math.Pow(p, 2) + Math.Pow(n, 2));
        }

        public static double ComputeArea(BiasVector a, BiasVector b)
        {
            int p = a.SignedStrength > 0 ? a.Magnitude : 0;
            int n = b.SignedStrength < 0 ? b.Magnitude : 0;
            return 0.5 * p * n;
        }

        public static string DescribeOverlay(BiasVector a, BiasVector b)
        {
            double hypotenuse = ComputeHypotenuse(a, b);
            double area = ComputeArea(a, b);
            return $"Overlay → Hypotenuse: {hypotenuse:F2}, Area: {area:F2}";
        }
    }
}
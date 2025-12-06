using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.structs;

namespace substrate_shared.Overlays
{
    public static class TrigOverlay
    {
        public static double AverageHypotenuse(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            var values = duels.Select(d => GeometryOverlay.ComputeHypotenuse(d.Item1, d.Item2));
            return values.Any() ? values.Average() : 0;
        }

        public static double CumulativeArea(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            return duels.Sum(d => GeometryOverlay.ComputeArea(d.Item1, d.Item2));
        }

        public static double MeanCos(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            var values = duels.Select(d =>
            {
                double hyp = GeometryOverlay.ComputeHypotenuse(d.Item1, d.Item2);
                return hyp > 0 ? d.Item1.Magnitude / hyp : 0;
            });
            return values.Any() ? values.Average() : 0;
        }

        public static double MeanSin(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            var values = duels.Select(d =>
            {
                double hyp = GeometryOverlay.ComputeHypotenuse(d.Item1, d.Item2);
                return hyp > 0 ? d.Item2.Magnitude / hyp : 0;
            });
            return values.Any() ? values.Average() : 0;
        }

        public static double LogScaledIndex(int resilienceIndex)
        {
            return Math.Log(Math.Abs(resilienceIndex) + 1);
        }

        public static double ExpScaledIndex(int resilienceIndex)
        {
            return Math.Exp(resilienceIndex / 10.0);
        }
    }
}
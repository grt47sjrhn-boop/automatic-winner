using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.structs;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Overlays;

namespace substrate_core.Overlays
{
    /// <summary>
    /// Manager responsible for trigonometric overlay calculations.
    /// Converts duel bias vectors into narratable aggregate metrics.
    /// </summary>
    public class TrigOverlay : ITrigOverlay
    {
        private readonly IGeometryOverlay _geometryOverlay;

        // 🔹 Inject GeometryOverlay for hypotenuse/area calculations
        public TrigOverlay(IGeometryOverlay geometryOverlay)
        {
            _geometryOverlay = geometryOverlay;
        }

        public double AverageHypotenuse(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            var values = duels.Select(d => _geometryOverlay.ComputeHypotenuse(d.Item1, d.Item2));
            return values.Any() ? values.Average() : 0;
        }

        public double CumulativeArea(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            return duels.Sum(d => _geometryOverlay.ComputeArea(d.Item1, d.Item2));
        }

        public double MeanCos(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            var values = duels.Select(d =>
            {
                var hyp = _geometryOverlay.ComputeHypotenuse(d.Item1, d.Item2);
                var ratio = hyp > 0 ? d.Item1.Magnitude / hyp : 0;
                return Math.Clamp(ratio, -1.0, 1.0);
            });
            return values.Any() ? values.Average() : 0;
        }

        public double MeanSin(IEnumerable<(BiasVector, BiasVector)> duels)
        {
            var values = duels.Select(d =>
            {
                var hyp = _geometryOverlay.ComputeHypotenuse(d.Item1, d.Item2);
                var ratio = hyp > 0 ? d.Item2.Magnitude / hyp : 0;
                return Math.Clamp(ratio, -1.0, 1.0);
            });
            return values.Any() ? values.Average() : 0;
        }

        public double LogScaledIndex(int resilienceIndex)
        {
            return Math.Log(Math.Abs(resilienceIndex) + 1);
        }

        public double ExpScaledIndex(int resilienceIndex)
        {
            return Math.Exp(resilienceIndex / 10.0);
        }
    }
}
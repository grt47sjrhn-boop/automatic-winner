using System;
using substrate_core.Utilities;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    public class DeltaVectorResolver : IResolver
    {
        public VectorBias Resolve(VectorBias vb, Mood im)
        {
            var deltaAxis = im.MoodAxis - vb.MoodAxis;

            vb.Hypotenuse = DebugOverlay.SafeFloat((float)Math.Sqrt(Math.Pow(deltaAxis, 2) + Math.Pow(im.Magnitude, 2)));
            vb.Area = DebugOverlay.SafeFloat(deltaAxis * im.Magnitude);
            vb.AngleTheta = DebugOverlay.SafeFloat((float)Math.Atan2(im.Magnitude, deltaAxis));

            vb.SinTheta = DebugOverlay.SafeFloat((float)Math.Sin(vb.AngleTheta));
            vb.CosTheta = DebugOverlay.SafeFloat((float)Math.Cos(vb.AngleTheta));
            vb.TanTheta = DebugOverlay.SafeFloat(DebugOverlay.Clamp((float)Math.Tan(vb.AngleTheta), -100f, 100f));

            Console.WriteLine($"[DeltaVectorResolver] ΔAxis={deltaAxis:F2}, Mag={im.Magnitude:F2}, " +
                              $"Hyp={vb.Hypotenuse:F2}, Area={vb.Area:F2}, Angle={vb.AngleTheta:F2}, " +
                              $"Sin={vb.SinTheta:F2}, Cos={vb.CosTheta:F2}, Tan={vb.TanTheta:F2}");

            return vb;
        }
    }
}
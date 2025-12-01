using System;
using System.Linq;
using substrate_core.Utilities;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    public class PersistenceResolver : IResolver
    {
        public VectorBias Resolve(VectorBias vb, Mood im)
        {
            float traitWeightSum = vb.Traits?.Sum(t => DebugOverlay.SafeFloat(t.Weight)) ?? 0;

            const float areaFactor = 0.3f;
            const float hypotenuseFactor = 0.4f;
            const float angleFactor = 0.2f;
            const float traitFactor = 0.1f;

            float rawPersistence =
                (vb.Area * areaFactor) +
                (vb.Hypotenuse * hypotenuseFactor) +
                (1 - Math.Abs(vb.AngleTheta) / (float)Math.PI) * angleFactor +
                (traitWeightSum * traitFactor) -
                vb.LogPressure;

            vb.Persistence = DebugOverlay.Clamp(rawPersistence, -100f, 100f);

            float logArg = 1 + vb.Persistence;
            vb.LogPressure = (logArg <= 0) ? 0f : (float)Math.Log(logArg);

            float expArg = vb.Persistence * vb.DriftMagnitude;
            vb.ExpVolatility = (float)Math.Exp(DebugOverlay.Clamp(expArg, -50f, 50f));

            DebugOverlay.LogResolver(nameof(PersistenceResolver), vb);

            return vb;
        }
    }
}
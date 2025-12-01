using System;
using System.Linq;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    public class IntentActionResolver : IResolver
    {
        public VectorBias Resolve(VectorBias vb, Mood mv)
        {
            vb.Intent = IntentType.None;

            if (vb.ToneTuple.Equals(default(ToneTuple)))
                return vb;

            bool hasDuality = vb.Traits?.Any(t => t.Tags.Contains(TraitTag.Duality)) ?? false;

            float persistence = DebugOverlay.SafeFloat(vb.Persistence);
            float volatility = DebugOverlay.SafeFloat(vb.ExpVolatility);
            float area = DebugOverlay.SafeFloat(vb.Area);

            if (vb.ToneTuple.Primary == Tone.Resonance && persistence > 2.5f)
                vb.Intent = IntentType.Stabilize;
            else if (vb.ToneTuple.Primary == Tone.Scar && volatility > 1.2f)
                vb.Intent = IntentType.Disrupt;
            else if (vb.ToneTuple.Primary == Tone.Neutral && hasDuality)
                vb.Intent = IntentType.Reflect;
            else if (vb.ToneTuple.Primary == Tone.Harmony && area > 10f)
                vb.Intent = IntentType.Amplify;

            Console.WriteLine($"[IntentActionResolver] Tone={vb.ToneTuple.Primary}, " +
                              $"Persistence={persistence:F2}, Volatility={volatility:F2}, Area={area:F2}, " +
                              $"Duality={hasDuality}, Intent={vb.Intent}");

            return vb;
        }
    }
}
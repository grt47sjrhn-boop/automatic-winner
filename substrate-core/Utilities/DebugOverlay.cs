// substrate_core/Utilities/DebugOverlay.cs

using System;
using substrate_shared.types.models;

namespace substrate_core.Utilities
{
    public static class DebugOverlay
    {
        // Safe math helpers
        public static float SafeFloat(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                return 0f;
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        // Resolver debug logging
        public static void LogResolver(string resolverName, VectorBias vb)
        {
            Console.WriteLine($"[{resolverName}] " +
                              $"Persistence={vb.Persistence:F2}, Volatility={vb.Volatility:F2}, " +
                              $"Area={vb.Area:F2}, Hyp={vb.Hypotenuse:F2}, Angle={vb.AngleTheta:F2}");
        }

        public static void LogTrigger(VectorBias vb, float crystallizationScore)
        {
            Console.WriteLine($"[TriggerResolver] Hyp={vb.Hypotenuse:F2}, Area={vb.Area:F2}, " +
                              $"Score={crystallizationScore:F2}, Events={vb.TriggerEvents.Count}");
            foreach (var evt in vb.TriggerEvents)
            {
                string scoreText = float.IsNaN(evt.Score) ? "NaN" : evt.Score.ToString("F2");
                string magText = float.IsNaN(evt.Magnitude) ? "NaN" : evt.Magnitude.ToString("F2");
                Console.WriteLine($"  Event: {evt.Type}, Score={scoreText}, Magnitude={magText}, Desc={evt.Description}");
            }
        }
    }
}
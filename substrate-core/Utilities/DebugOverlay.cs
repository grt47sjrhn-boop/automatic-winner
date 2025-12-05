// substrate_core/Utilities/DebugOverlay.cs

using System;
using substrate_core.Resolvers;
using substrate_shared.types.models;
using substrate_shared.types.Summaries;

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
            Console.WriteLine($"[{resolverName}]");

            foreach (var summary in vb.Summaries.Values)
            {
                Console.WriteLine("  " + summary.Describe());
            }
        }


        public static void LogTrigger(VectorBias vb, float crystallizationScore, DeltaSummary delta, TriggerSummary triggers)
        {
            Console.WriteLine($"[TriggerResolver] Hyp={delta.Hypotenuse:F2}, Area={delta.Area:F2}, " +
                              $"Score={crystallizationScore:F2}, Events={triggers.Events.Count}");
            foreach (var evt in triggers.Events)
            {
                var scoreText = float.IsNaN(evt.Score) ? "NaN" : evt.Score.ToString("F2");
                var magText = float.IsNaN(evt.Magnitude) ? "NaN" : evt.Magnitude.ToString("F2");
                Console.WriteLine($"  Event: {evt.Type}, Score={scoreText}, Magnitude={magText}, Desc={evt.Description}");
            }
        }
    }
}
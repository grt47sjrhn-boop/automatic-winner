using System;
using System.Collections.Generic;

namespace substrate_shared.Types.Modles
{
    public class TrendAnalyser
    {
        public void ShowBiasTrends(BiasProfile profile)
        {
            Console.WriteLine("\n=== Bias Trend Analysis ===");

            foreach (var entry in profile.Modifiers)
            {
                Console.WriteLine($"Reason: {entry.Key}");
                foreach (var (timestamp, delta) in entry.Value)
                {
                    Console.WriteLine($"  {timestamp}: {delta:+0.00;-0.00}");
                }
            }

            Console.WriteLine("\nCurrent Bias Levels:");
            Console.WriteLine($"Fear: {profile.FearLevel:0.00}");
            Console.WriteLine($"Greed: {profile.GreedLevel:0.00}");
            Console.WriteLine($"Loyalty: {profile.LoyaltyLevel:0.00}");
            Console.WriteLine($"Distrust: {profile.DistrustLevel:0.00}");

            Console.WriteLine($"\nDominant Bias: {profile.DominantBias} ({profile.DominantLevel:0.00})");
        }

        public void ShowTickCycleSummary(List<BiasProfile> tickProfiles)
        {
            Console.WriteLine("\n=== Tick Cycle Summary ===");
            var tick = 1;
            foreach (var profile in tickProfiles)
            {
                Console.WriteLine($"Tick {tick++}: Dominant = {profile.DominantBias} ({profile.DominantLevel:0.00})");
            }
        }
    }
}
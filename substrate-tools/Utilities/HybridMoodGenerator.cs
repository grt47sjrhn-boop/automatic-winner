using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_tools.Utilities
{
    public static class HybridMoodGenerator
    {
        public static IEnumerable<Mood> GenerateHybridSequence(int randomCount)
        {
            var moods = new List<Mood>();

            // Scripted beats (axis only, volatility defaults to 1.0f)
            var scar = new Mood();
            scar.SetMood(MoodType.Sadness);   // Axis auto-syncs to -9
            moods.Add(scar);

            var resonance = new Mood();
            resonance.SetMood(MoodType.Optimism); // Axis auto-syncs to 4
            moods.Add(resonance);

            var harmony = new Mood();
            harmony.SetMood(MoodType.Joy);    // Axis auto-syncs to 8
            moods.Add(harmony);

            // Random filler
            moods.AddRange(RandomMoodGenerator.GenerateSequence(randomCount));

            return moods;
        }
    }
}
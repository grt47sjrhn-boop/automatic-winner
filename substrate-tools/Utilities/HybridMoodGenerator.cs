using System.Collections.Generic;
using substrate_shared.types.structs;

namespace substrate_tools.Utilities
{
    public static class HybridMoodGenerator
    {
        public static IEnumerable<Mood> GenerateHybridSequence(int randomCount)
        {
            var moods = new List<Mood>();

            // Scripted beats
            moods.Add(new Mood { MoodAxis = -9, Magnitude = 3.5f, ClusterId = "ScarBeat" });
            moods.Add(new Mood { MoodAxis = 4, Magnitude = 2.0f, ClusterId = "ResonanceBeat" });
            moods.Add(new Mood { MoodAxis = 8, Magnitude = 4.0f, ClusterId = "HarmonyBeat" });

            // Random filler
            moods.AddRange(RandomMoodGenerator.GenerateSequence(randomCount));

            return moods;
        }
    }
}
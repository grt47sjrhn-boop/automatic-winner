using System;
using System.Collections.Generic;
using substrate_shared.types.structs;

namespace substrate_tools.Utilities
{
    public static class RandomMoodGenerator
    {
        private static readonly Random _rng = new Random();

        // Generate a single random Mood
        public static Mood GenerateMood()
        {
            // Pick a random axis between -11 and +11
            var axis = _rng.Next(-11, 12);

            var mood = new Mood();
            mood.MoodAxis = axis;      // auto-syncs MoodType

            return mood;
        }

        // Generate a sequence of random Moods
        public static IEnumerable<Mood> GenerateSequence(int count)
        {
            var moods = new List<Mood>();
            for (var i = 0; i < count; i++)
                moods.Add(GenerateMood());
            return moods;
        }
    }
}
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
            return new Mood
            {
                MoodAxis = _rng.Next(-11, 12), // inclusive range -11 … +11
                Magnitude = (float)(_rng.NextDouble() * 5.0), // 0.0 … 5.0
                ClusterId = $"Cluster{_rng.Next(1, 100)}"
            };
        }

        // Generate a sequence of random Moods
        public static IEnumerable<Mood> GenerateSequence(int count)
        {
            var moods = new List<Mood>();
            for (int i = 0; i < count; i++)
            {
                moods.Add(GenerateMood());
            }
            return moods;
        }
    }
}
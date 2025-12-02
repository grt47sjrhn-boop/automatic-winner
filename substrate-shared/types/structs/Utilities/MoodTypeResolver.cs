using System;
using substrate_shared.enums;

namespace substrate_core.Resolvers
{
    public static class MoodTypeResolver
    {
        /// <summary>
        /// Resolves a float MoodAxis value (-11.0 … +11.0) to the closest MoodType enum.
        /// </summary>
        public static MoodType Resolve(float axis)
        {
            float clamped = MathF.Max(-11f, MathF.Min(11f, axis));
            int rounded = (int)MathF.Round(clamped);

            if (Enum.IsDefined(typeof(MoodType), rounded))
                return (MoodType)rounded;

            return MoodType.Neutral;
        }

        /// <summary>
        /// Returns a tuple containing the current MoodType, its adjacent values, and its opposite.
        /// </summary>
        public static (MoodType Current, MoodType? Previous, MoodType? Next, MoodType? Opposite) ResolveWithContext(float axis)
        {
            // Current mood
            MoodType current = Resolve(axis);

            int rounded = (int)MathF.Round(MathF.Max(-11f, MathF.Min(11f, axis)));

            // Adjacent moods
            MoodType? prev = Enum.IsDefined(typeof(MoodType), rounded - 1)
                ? (MoodType)(rounded - 1)
                : null;

            MoodType? next = Enum.IsDefined(typeof(MoodType), rounded + 1)
                ? (MoodType)(rounded + 1)
                : null;

            // Opposite mood (mirrored across axis)
            int opposite = -rounded;
            MoodType? opp = Enum.IsDefined(typeof(MoodType), opposite)
                ? (MoodType)opposite
                : null;

            return (current, prev, next, opp);
        }
    }
}
using System;
using substrate_shared.enums;
using substrate_core.Resolvers;

namespace substrate_shared.types.structs
{
    public struct Mood
    {
        private float _moodAxis;

        // Continuous emotional axis: -11.0 (Despair) to +11.0 (Ecstasy)
        public float MoodAxis
        {
            get => _moodAxis;
            set
            {
                _moodAxis = value;
                MoodType = MoodTypeResolver.Resolve(_moodAxis);
            }
        }

        // Human-readable label (Sadness, Joy, etc.)
        public MoodType MoodType { get; private set; }

        // Calculate vector distance between this mood and another (axis only)
        public float MagnitudeFrom(Mood other)
        {
            return MathF.Abs(MoodAxis - other.MoodAxis);
        }

        /// <summary>
        /// Sets the mood directly by MoodType.
        /// Automatically syncs MoodAxis to the enum’s numeric value.
        /// </summary>
        public void SetMood(MoodType type)
        {
            MoodType = type;
            MoodAxis = (int)type; // enum values map directly to axis integers
        }

        public override string ToString()
        {
            return $"{MoodType} (Axis={MoodAxis:F1})";
        }
    }
}
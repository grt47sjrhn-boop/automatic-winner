using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_shared.Factories
{
    public static class MoodFactory
    {
        /// <summary>
        /// Creates a Mood from a MoodType and given volatility.
        /// Axis is derived directly from the enum’s numeric value.
        /// </summary>
        public static Mood Create(MoodType type, float volatility = 0f)
        {
            var mood = new Mood();
            mood.SetMood(type);
            return mood;
        }
    }
}
namespace substrate_shared.enums
{
    public enum MoodType
    {
        [MoodDescription("Utter hopelessness, the deepest negative state.", "Negative", "Abyssal States")]
        Despair = -11,

        [MoodDescription("Profound sorrow and mourning.", "Negative", "Abyssal States")]
        Grief = -10,

        [MoodDescription("Lingering unhappiness or melancholy.", "Negative", "Shadow States")]
        Sadness = -9,

        [MoodDescription("Quiet, reflective sadness.", "Negative", "Shadow States")]
        Melancholy = -8,

        [MoodDescription("Dark, heavy emotional weight.", "Negative", "Shadow States")]
        Gloom = -7,

        [MoodDescription("Persistent unease and worry.", "Negative", "Anxious Currents")]
        Anxiety = -6,

        [MoodDescription("Concern and apprehension.", "Negative", "Anxious Currents")]
        Worry = -5,

        [MoodDescription("Mild discomfort or restlessness.", "Negative", "Fractured Tensions")]
        Unease = -4,

        [MoodDescription("Agitation, difficulty finding calm.", "Negative", "Fractured Tensions")]
        Restlessness = -3,

        [MoodDescription("Annoyance or frustration.", "Negative", "Fractured Tensions")]
        Irritation = -2,

        [MoodDescription("Low-level dissatisfaction.", "Negative", "Fractured Tensions")]
        Discomfort = -1,

        [MoodDescription("Balanced, neither positive nor negative.", "Neutral", "Equilibrium")]
        Neutral = 0,

        [MoodDescription("Mild satisfaction and ease.", "Positive", "Gentle Radiance")]
        Contentment = 1,

        [MoodDescription("Relaxed, free from worry.", "Positive", "Gentle Radiance")]
        Ease = 2,

        [MoodDescription("Expectation of positive outcomes.", "Positive", "Rising Hope")]
        Hope = 3,

        [MoodDescription("Confidence in favorable possibilities.", "Positive", "Rising Hope")]
        Optimism = 4,

        [MoodDescription("Heightened anticipation and energy.", "Positive", "Bright Currents")]
        Excitement = 5,

        [MoodDescription("Looking forward with eagerness.", "Positive", "Bright Currents")]
        Anticipation = 6,

        [MoodDescription("Joyful pleasure and satisfaction.", "Positive", "Core Joy")]
        Delight = 7,

        [MoodDescription("General positive emotional state.", "Positive", "Core Joy")]
        Happiness = 8,

        [MoodDescription("Strong, uplifting happiness.", "Positive", "Core Joy")]
        Joy = 9,

        [MoodDescription("Exuberant, soaring positivity.", "Positive", "Transcendent Peaks")]
        Elation = 10,

        [MoodDescription("Intense bliss, peak positive state.", "Positive", "Transcendent Peaks")]
        Ecstasy = 11
    }
}
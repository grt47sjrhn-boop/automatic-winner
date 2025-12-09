using substrate_shared.Registries.enums.Attributes;

namespace substrate_shared.Registries.enums
{
    public enum MoodType
    {
        // --- Abyssal States ---
        [RegistryNarrative("Despair: a black pit where hope is strangled, silence heavier than stone.", Bias.Negative, NarrativeGroup.AbyssalStates, scaleValue: -11)]
        Despair = -11,

        [RegistryNarrative("Grief: funeral cloth drapes the soul, every breath a dirge in endless night.", Bias.Negative, NarrativeGroup.AbyssalStates, scaleValue: -10)]
        Grief = -10,

        // --- Shadow States ---
        [RegistryNarrative("Sadness: rain lingering on forgotten graves, a chill that never lifts.", Bias.Negative, NarrativeGroup.ShadowStates, scaleValue: -9)]
        Sadness = -9,

        [RegistryNarrative("Melancholy: a candle guttering low, corridors whispering with dim echoes.", Bias.Negative, NarrativeGroup.ShadowStates, scaleValue: -8)]
        Melancholy = -8,

        [RegistryNarrative("Gloom: dust settling in abandoned halls, shadows thick as tar.", Bias.Negative, NarrativeGroup.ShadowStates, scaleValue: -7)]
        Gloom = -7,

        // --- Anxious Currents ---
        [RegistryNarrative("Anxiety: claws at the edges of thought, restless shadows pacing unseen.", Bias.Negative, NarrativeGroup.AnxiousCurrents, scaleValue: -6)]
        Anxiety = -6,

        [RegistryNarrative("Worry: rats gnawing in the walls, unseen but relentless.", Bias.Negative, NarrativeGroup.AnxiousCurrents, scaleValue: -5)]
        Worry = -5,

        // --- Fractured Tensions ---
        [RegistryNarrative("Unease: footsteps behind you in the dark, never quite catching up.", Bias.Negative, NarrativeGroup.FracturedTensions, scaleValue: -4)]
        Unease = -4,

        [RegistryNarrative("Restlessness: chains rattling in the mind, no corner offering peace.", Bias.Negative, NarrativeGroup.FracturedTensions, scaleValue: -3)]
        Restlessness = -3,

        [RegistryNarrative("Irritation: sparks of broken glass underfoot, sharp and sudden.", Bias.Negative, NarrativeGroup.FracturedTensions, scaleValue: -2)]
        Irritation = -2,

        [RegistryNarrative("Discomfort: a discordant hum beneath the skin, low and unyielding.", Bias.Negative, NarrativeGroup.FracturedTensions, scaleValue: -1)]
        Discomfort = -1,

        // --- Equilibrium ---
        [RegistryNarrative("Neutral: still water, neither storm nor calm, a hush before revelation.", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
        Neutral = 0,

        // --- Gentle Radiance ---
        [RegistryNarrative("Contentment: a hearth’s glow after wandering, warmth against the cold void.", Bias.Positive, NarrativeGroup.GentleRadiance, scaleValue: 1)]
        Contentment = 1,

        [RegistryNarrative("Ease: fog drifting soft, quieting the world’s jagged edges.", Bias.Positive, NarrativeGroup.GentleRadiance, scaleValue: 2)]
        Ease = 2,

        // --- Rising Hope ---
        [RegistryNarrative("Hope: dawn through cracked shutters, fragile light against the abyss.", Bias.Positive, NarrativeGroup.RisingHope, scaleValue: 3)]
        Hope = 3,

        [RegistryNarrative("Optimism: lanterns lit against the night, humming with defiance.", Bias.Positive, NarrativeGroup.RisingHope, scaleValue: 4)]
        Optimism = 4,

        // --- Bright Currents ---
        [RegistryNarrative("Excitement: thunder before the storm breaks, air charged and trembling.", Bias.Positive, NarrativeGroup.BrightCurrents, scaleValue: 5)]
        Excitement = 5,

        [RegistryNarrative("Anticipation: a bowstring drawn tight, waiting for release.", Bias.Positive, NarrativeGroup.BrightCurrents, scaleValue: 6)]
        Anticipation = 6,

        // --- Core Joy ---
        [RegistryNarrative("Delight: firelight dancing on glass, fleeting but radiant.", Bias.Positive, NarrativeGroup.CoreJoy, scaleValue: 7)]
        Delight = 7,

        [RegistryNarrative("Happiness: wildflowers blooming through cracked stone, stubborn and bright.", Bias.Positive, NarrativeGroup.CoreJoy, scaleValue: 8)]
        Happiness = 8,

        [RegistryNarrative("Joy: bells tolling across a city at dawn, echoing through ruins.", Bias.Positive, NarrativeGroup.CoreJoy, scaleValue: 9)]
        Joy = 9,

        // --- Transcendent Peaks ---
        [RegistryNarrative("Elation: wings soaring over ruined towers, defiance in flight.", Bias.Positive, NarrativeGroup.TranscendentPeaks, scaleValue: 10)]
        Elation = 10,

        [RegistryNarrative("Ecstasy: stars burning too close to touch, brilliance that consumes.", Bias.Positive, NarrativeGroup.TranscendentPeaks, scaleValue: 11)]
        Ecstasy = 11
    }
}
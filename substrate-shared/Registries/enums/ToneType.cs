using substrate_shared.Registries.enums.Attributes;

namespace substrate_shared.Registries.enums
{
    public enum ToneType
    {
        // 🌞 Positive tones
        [RegistryNarrative("Joy rings...", Bias.Positive, NarrativeGroup.CoreJoy, scaleValue: 1)]
        Joy,

        [RegistryNarrative("Playful shadows...", Bias.Positive, NarrativeGroup.CoreJoy, scaleValue: 2)]
        Playful,

        [RegistryNarrative("Resilience shines...", Bias.Positive, NarrativeGroup.Crystal, scaleValue: 3)]
        Resilient,

        [RegistryNarrative("Radiance bursts...", Bias.Positive, NarrativeGroup.Crystal, scaleValue: 4)]
        Radiant,

        // ⚖️ Neutral tones
        [RegistryNarrative("Neutral silence...", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
        Neutral,

        [RegistryNarrative("Calm descends...", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
        Calm,

        [RegistryNarrative("Reflective lantern...", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
        Reflective,

        [RegistryNarrative("Harmony hums...", Bias.Neutral, NarrativeGroup.Crystal, scaleValue: 0)]
        Harmonious,

        [RegistryNarrative("Composite duel tone...", Bias.Neutral, NarrativeGroup.Composite, scaleValue: 0)]
        Composite,

        [RegistryNarrative("Equilibrium whispers...", Bias.Neutral, NarrativeGroup.Crystal, scaleValue: 0)]
        Equilibrium,

        // 🔥 Mixed tones
        [RegistryNarrative("Conflict reverberates...", Bias.Mixed, NarrativeGroup.Crystal, scaleValue: -1)]
        Conflict,

        // 🌑 Negative tones (existing)
        [RegistryNarrative("Wound bleeds...", Bias.Negative, NarrativeGroup.Crystal, scaleValue: -2)]
        Wound,

        [RegistryNarrative("Despairing tones...", Bias.Negative, NarrativeGroup.AbyssalStates, scaleValue: -3)]
        Despairing,

        [RegistryNarrative("Hostile burns...", Bias.Negative, NarrativeGroup.HostileActions, scaleValue: -1)]
        Hostile,

        [RegistryNarrative("Critical cuts...", Bias.Negative, NarrativeGroup.HostileActions, scaleValue: -1)]
        Critical,

        // 🌌 Extended abyssal tones (new)
        [RegistryNarrative("Forsaken echoes...", Bias.Negative, NarrativeGroup.AbyssalStates, scaleValue: -4)]
        Forsaken,

        [RegistryNarrative("Corrupted resonance...", Bias.Negative, NarrativeGroup.AbyssalStates, scaleValue: -5)]
        Corrupted,

        [RegistryNarrative("Doomed silence...", Bias.Negative, NarrativeGroup.AbyssalStates, scaleValue: -6)]
        Doomed
    }
}
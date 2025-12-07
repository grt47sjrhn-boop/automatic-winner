using substrate_shared.Registries.enums;
using substrate_shared.Registries.enums.Attributes;

namespace substrate_shared.Registries.enums
{
    public enum ToneType
    {
        [RegistryNarrative("Joy rings like bells in a haunted city, fragile yet defiant against the void.", Bias.Positive, NarrativeGroup.CoreJoy)]
        Joy,

        [RegistryNarrative("Playful shadows dance at the edge of sanity, laughter echoing in forgotten halls.", Bias.Positive, NarrativeGroup.CoreJoy)]
        Playful,

        [RegistryNarrative("Calm descends like mist over a graveyard, still waters hiding unfathomable depths.", Bias.Neutral, NarrativeGroup.Equilibrium)]
        Calm,

        [RegistryNarrative("Neutral silence, the hush before revelation, neither salvation nor damnation.", Bias.Neutral, NarrativeGroup.Equilibrium)]
        Neutral,

        [RegistryNarrative("Reflection stirs like a lantern in the abyss, casting fleeting shapes on cavern walls.", Bias.Neutral, NarrativeGroup.Equilibrium)]
        Reflective,

        [RegistryNarrative("Hostility burns like black stars, a corrosive fire that devours all bonds.", Bias.Negative, NarrativeGroup.HostileActions)]
        Hostile,

        [RegistryNarrative("Criticism cuts like rusted iron, sharp words echoing in chambers of doubt.", Bias.Negative, NarrativeGroup.HostileActions)]
        Critical,

        [RegistryNarrative("Despairing tones seep like seawater into cracked stone, a tide that cannot be turned.", Bias.Negative, NarrativeGroup.AbyssalStates)]
        Despairing,
        
        // 🔹 New synthetic entries for multi-axis duels
        [RegistryNarrative("Composite duel tone, forged from multiple axes into a singular resonance.", Bias.Neutral, NarrativeGroup.Composite)]
        Composite,

        [RegistryNarrative("Wound bleeds into the narrative, a scar etched in fragile flesh and haunted memory.", Bias.Negative, NarrativeGroup.Crystal)]
        Wound,

        [RegistryNarrative("Conflict reverberates like clashing iron in a cathedral of shadows, unresolved and eternal.", Bias.Mixed, NarrativeGroup.Crystal)]
        Conflict,

        [RegistryNarrative("Equilibrium whispers of fragile balance, a stasis trembling before collapse or recovery.", Bias.Neutral, NarrativeGroup.Crystal)]
        Equilibrium
    }
}
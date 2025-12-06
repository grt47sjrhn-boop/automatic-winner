using substrate_shared.Registries.enums.Attributes;

namespace substrate_shared.Registries.enums
{
    public enum NarrativeGroup
    {
        [Narrative("The abyss where hope drowns, heavy silence and funeral echoes.", Bias.Negative)]
        AbyssalStates,

        [Narrative("Shadows linger in forgotten halls, melancholy whispers and dim corridors.", Bias.Negative)]
        ShadowStates,

        [Narrative("Currents of unease, restless tides gnawing at the edges of thought.", Bias.Negative)]
        AnxiousCurrents,

        [Narrative("Fractured tensions, sparks and chains rattling in the dark.", Bias.Negative)]
        FracturedTensions,

        [Narrative("Equilibrium, the still water before revelation, neither storm nor calm.", Bias.Neutral)]
        Equilibrium,

        [Narrative("Gentle radiance, hearth‑light and fog that softens jagged edges.", Bias.Positive)]
        GentleRadiance,

        [Narrative("Rising hope, lanterns lit against the night, fragile dawn breaking.", Bias.Positive)]
        RisingHope,

        [Narrative("Bright currents, thunder before the storm, anticipation coiled tight.", Bias.Positive)]
        BrightCurrents,

        [Narrative("Core joy, firelight dancing, wildflowers blooming through cracked stone.", Bias.Positive)]
        CoreJoy,

        [Narrative("Transcendent peaks, wings soaring over ruins, stars burning too close to touch.", Bias.Positive)]
        TranscendentPeaks,

        [Narrative("Supportive actions, torches raised in the dark, hands extended across the abyss.", Bias.Positive)]
        SupportiveActions,

        [Narrative("Affirmative actions, laughter rising in ruins, visions glimpsed beyond mortal reach.", Bias.Positive)]
        AffirmativeActions,

        [Narrative("Hostile actions, black stars burning, corrosive fire devouring fragile bonds.", Bias.Negative)]
        HostileActions,

        [Narrative("Fractured actions, whispers leading astray, labyrinths of madness unfolding.", Bias.Negative)]
        FracturedActions,

        [Narrative("Abyssal actions, despairing voices, hopeless tides that cannot be turned.", Bias.Negative)]
        AbyssalActions,
        
        [Narrative("Composite synthesis, forged from multiple axes into a singular resonance.", Bias.Neutral)]
        Composite
    }
}
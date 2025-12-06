using substrate_shared.Registries.enums.Attributes;

namespace substrate_shared.Registries.enums
{
    public enum IntentAction
    {
        // --- Positive -----------------------------------------------------
        [RegistryNarrative("Encourage: a torch raised in the dark, fragile flame against endless night.", Bias.Positive, NarrativeGroup.SupportiveActions)]
        Encourage,

        [RegistryNarrative("Support: a hand extended across the abyss, steadying the trembling soul.", Bias.Positive, NarrativeGroup.SupportiveActions)]
        Support,

        [RegistryNarrative("Comfort: a whisper in the storm, soft words against the howling void.", Bias.Positive, NarrativeGroup.SupportiveActions)]
        Comfort,

        [RegistryNarrative("Approve: a nod beneath dim lanterns, fleeting warmth in a cold hall.", Bias.Positive, NarrativeGroup.AffirmativeActions)]
        Approve,

        [RegistryNarrative("Inspire: a vision glimpsed in dream, stars burning beyond mortal reach.", Bias.Positive, NarrativeGroup.AffirmativeActions)]
        Inspire,

        [RegistryNarrative("Celebrate: laughter rising like smoke, defiant joy in crumbling ruins.", Bias.Positive, NarrativeGroup.AffirmativeActions)]
        Celebrate,

        // --- Neutral ------------------------------------------------------
        [RegistryNarrative("Observe: eyes unblinking, watching the tide of fate without judgment.", Bias.Neutral, NarrativeGroup.Equilibrium)]
        Observe,

        [RegistryNarrative("Inform: words etched like runes, cold clarity carved into stone.", Bias.Neutral, NarrativeGroup.Equilibrium)]
        Inform,

        [RegistryNarrative("Question: a voice in the dark corridor, asking what lurks beyond the door.", Bias.Neutral, NarrativeGroup.Equilibrium)]
        Question,

        [RegistryNarrative("Reflect: a mirror cracked, showing truths both comforting and monstrous.", Bias.Neutral, NarrativeGroup.Equilibrium)]
        Reflect,

        // --- Negative -----------------------------------------------------
        [RegistryNarrative("Criticize: a blade of thought, cutting deep into fragile hope.", Bias.Negative, NarrativeGroup.HostileActions)]
        Criticize,

        [RegistryNarrative("Challenge: a gauntlet thrown in the dust, daring the abyss to answer.", Bias.Negative, NarrativeGroup.HostileActions)]
        Challenge,

        [RegistryNarrative("Reject: a door slammed in the storm, leaving the wanderer outside.", Bias.Negative, NarrativeGroup.HostileActions)]
        Reject,

        [RegistryNarrative("Distract: whispers that lead astray, paths winding into labyrinths of madness.", Bias.Negative, NarrativeGroup.FracturedActions)]
        Distract,

        [RegistryNarrative("Cynical: laughter hollow as tombs, mocking every fragile dream.", Bias.Negative, NarrativeGroup.HostileActions)]
        Cynical
    }
}
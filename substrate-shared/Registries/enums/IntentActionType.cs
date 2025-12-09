using substrate_shared.Registries.enums;
using substrate_shared.Registries.enums.Attributes;

public enum IntentAction
{
    // --- Positive -----------------------------------------------------
    [RegistryNarrative("Encourage: a torch raised in the dark...", Bias.Positive, NarrativeGroup.SupportiveActions, scaleValue: 1)]
    Encourage,

    [RegistryNarrative("Support: a hand extended across the abyss...", Bias.Positive, NarrativeGroup.SupportiveActions, scaleValue: 2)]
    Support,

    [RegistryNarrative("Comfort: a whisper in the storm...", Bias.Positive, NarrativeGroup.SupportiveActions, scaleValue: 3)]
    Comfort,

    [RegistryNarrative("Approve: a nod beneath dim lanterns...", Bias.Positive, NarrativeGroup.AffirmativeActions, scaleValue: 4)]
    Approve,

    [RegistryNarrative("Inspire: a vision glimpsed in dream...", Bias.Positive, NarrativeGroup.AffirmativeActions, scaleValue: 5)]
    Inspire,

    [RegistryNarrative("Celebrate: laughter rising like smoke...", Bias.Positive, NarrativeGroup.AffirmativeActions, scaleValue: 6)]
    Celebrate,

    // --- Neutral ------------------------------------------------------
    [RegistryNarrative("Observe: eyes unblinking...", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
    Observe,

    [RegistryNarrative("Inform: words etched like runes...", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
    Inform,

    [RegistryNarrative("Question: a voice in the dark corridor...", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
    Question,

    [RegistryNarrative("Reflect: a mirror cracked...", Bias.Neutral, NarrativeGroup.Equilibrium, scaleValue: 0)]
    Reflect,

    // --- Negative -----------------------------------------------------
    [RegistryNarrative("Criticize: a blade of thought...", Bias.Negative, NarrativeGroup.HostileActions, scaleValue: -1)]
    Criticize,

    [RegistryNarrative("Challenge: a gauntlet thrown...", Bias.Negative, NarrativeGroup.HostileActions, scaleValue: -2)]
    Challenge,

    [RegistryNarrative("Reject: a door slammed...", Bias.Negative, NarrativeGroup.HostileActions, scaleValue: -3)]
    Reject,

    [RegistryNarrative("Distract: whispers that lead astray...", Bias.Negative, NarrativeGroup.FracturedActions, scaleValue: -4)]
    Distract,

    [RegistryNarrative("Cynical: laughter hollow as tombs...", Bias.Negative, NarrativeGroup.HostileActions, scaleValue: -5)]
    Cynical
}
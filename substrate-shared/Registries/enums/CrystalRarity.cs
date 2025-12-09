using substrate_shared.Registries.enums.Attributes;

namespace substrate_shared.Registries.enums;

public enum CrystalRarity
{
    [RegistryNarrative(
        "Common crystal: faint glimmers forged at low thresholds, fragile yet narratable.",
        Bias.Neutral,
        NarrativeGroup.Crystal,
        scaleValue: 0)]
    Common,

    [RegistryNarrative(
        "Rare crystal: resilience tempered, facets sharpened, a subtle brilliance against collapse.",
        Bias.Positive,
        NarrativeGroup.Crystal,
        scaleValue: 1)]
    Rare,

    [RegistryNarrative(
        "Epic crystal: radiant arcs forged in duels, brilliance echoing through fractured halls.",
        Bias.Positive,
        NarrativeGroup.Crystal,
        scaleValue: 2)]
    Epic,

    [RegistryNarrative(
        "Mythic crystal: forged in paradox, shimmering with dual resonance, a cadence between epic arcs and legendary collapse.",
        Bias.Mixed,
        NarrativeGroup.Crystal,
        scaleValue: 3)]
    Mythic,

    [RegistryNarrative(
        "Legendary crystal: transcendent resonance, stars burning too close to touch, mythic artifact of resilience.",
        Bias.Mixed,
        NarrativeGroup.Crystal,
        scaleValue: 4)]
    Legendary,

    [RegistryNarrative(
        "UltraRare crystal: shimmering beyond myth, a resonance so scarce it bends fate itself.",
        Bias.Positive,
        NarrativeGroup.Crystal,
        scaleValue: 5)]
    UltraRare,
    Fragile,
    Corrupted,
    Doomed
}
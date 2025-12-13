using System;
using System.Collections.Generic;
using substrate_shared.Registries.enums;

namespace substrate_shared.Records.Codex
{
    public abstract record Profile(
        Guid ContributorId,
        string DisplayName,
        Preset Preset,
        NarrativeFlavor Flavor,
        DateTime CreatedAt
    );

    public record Preset(
        string Difficulty,
        string Bias,
        string OverlaySet
    );

    public record Lineage(
        Guid ContributorId,
        IEnumerable<Guid> Ancestors,
        string OriginPreset
    );

    public record AncestryMetadata(
        Guid ContributorId,
        Lineage Lineage,
        Preset CurrentPreset,
        NarrativeFlavor Flavor
    );
}
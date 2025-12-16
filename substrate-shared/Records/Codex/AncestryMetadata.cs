using System;
using substrate_shared.Registries.enums;

namespace substrate_shared.Records.Codex
{
    public record AncestryMetadata(
        Guid ContributorId,
        Lineage Lineage,
        Preset CurrentPreset,
        NarrativeFlavor Flavor
    );
}
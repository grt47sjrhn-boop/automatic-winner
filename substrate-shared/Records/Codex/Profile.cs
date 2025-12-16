using System;
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
}
using System;
using System.Collections.Generic;

namespace substrate_shared.Records.Codex
{
    public record Lineage(
        Guid ContributorId,
        IEnumerable<Guid> Ancestors,
        string OriginPreset
    );
}
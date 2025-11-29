using System;
using substrate_shared.enums;
using substrate_shared.structs;
using substrate_shared.types.core;

public sealed class ResolvedArtifact
{
    public ArtifactType Type { get; }
    public Trait Trait { get; }
    public double BaseChance { get; }
    public double Modifier { get; set; }
    public string Outcome { get; set; }

    public ResolvedArtifact(ArtifactType type, Trait trait, double baseChance)
    {
        Type = type;
        Trait = trait;
        BaseChance = baseChance;
        Modifier = 0.0;
        Outcome = string.Empty;
    }

    public double EffectiveChance => Math.Max(0.0, Math.Min(1.0, BaseChance + Modifier));

    // Dual conversion
    public ArtifactCore ToCore() => new ArtifactCore(Type);
    public Artifact ToArtifact() => new Artifact(Type, BaseChance, Trait);
}
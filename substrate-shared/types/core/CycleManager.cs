using System;
using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.extensions;
using substrate_shared.structs;
using substrate_shared.types.core;

public sealed class CycleManager
{
    private readonly List<ITrigger> _triggers = new();
    private readonly IArtifactHook _artifactHook;
    private readonly IBiasDriftHook _biasHook;
    private readonly ILoreLogger _lore;

    public CycleManager(IArtifactHook artifactHook, IBiasDriftHook biasHook, ILoreLogger lore)
    {
        _artifactHook = artifactHook;
        _biasHook = biasHook;
        _lore = lore;

        _triggers.Add(new Trigger("T-ResolveTriangle", ctx => ctx.Magnitude >= 0.4f, 0.56, "Triangle resolution achieved"));
        _triggers.Add(new Trigger("T-BelowMagnitude", ctx => ctx.Magnitude < 0.4f, 0.00, "Magnitude below threshold"));
        _triggers.Add(new Trigger("T-Collapse", ctx => ctx.Pressure > 1.0, 1.12, "Collapse crystallized"));
    }

    public void RunCycle(CycleContext ctx, IReadOnlyList<ResolvedArtifact> candidates)
    {
        // Bias drift hook
        _biasHook.OnBiasDrift(ctx.OldBias, ctx.NewBias);

        // Fire triggers
        foreach (var t in _triggers) t.Execute(ctx);

        // Adjust rarity modifiers
        double collapseBoost = HasTrigger("T-Collapse", ctx) ? 0.05 : 0.0;
        double triangleBoost = HasTrigger("T-ResolveTriangle", ctx) ? 0.03 : 0.0;
        double quietPenalty = HasTrigger("T-BelowMagnitude", ctx) ? -0.02 : 0.0;

        foreach (var a in candidates)
        {
            switch (a.Type)
            {
                case ArtifactType.Pearl:
                    a.Modifier = triangleBoost + quietPenalty;
                    break;
                case ArtifactType.RarePearl:
                    a.Modifier = triangleBoost * 0.5;
                    break;
                case ArtifactType.ChaosCrystal:
                    a.Modifier = collapseBoost;
                    break;
                case ArtifactType.CatastrophicCollapse:
                    a.Modifier = collapseBoost * 0.8;
                    break;
                case ArtifactType.SilentCycle:
                    a.Modifier = quietPenalty;
                    break;
            }
        }

        // Resolve one artifact
        var resolved = WeightedPick(candidates);

        // Dual outputs
        ArtifactCore artifactCore = resolved.ToCore();
        Artifact toArtifact = resolved.ToArtifact();


        // Add to context list (IArtifact)
        ctx.Artifacts.Add(toArtifact);

        // Lore log
        _lore.LogCycle(
            input: new MoodVector(
                ctx.NewBias.Resonance,
                ctx.NewBias.Warmth,
                ctx.NewBias.Irony,
                ctx.NewBias.Flux,
                ctx.NewBias.Inertia,
                ctx.NewBias.Variance,
                ctx.NewBias.HiddenPressure),
            delta: ctx.BiasDelta,
            magnitude: ctx.Magnitude,
            layers: ctx.Layers,
            artifact: artifactCore,
            trait: resolved.Trait);

        // Artifact hook
        _artifactHook.OnArtifactResolved(toArtifact, ctx.NewBias);
    }

    private static bool HasTrigger(string id, CycleContext ctx) => ctx.FiredTriggers.Exists(t => t.Id == id);

    private static ResolvedArtifact WeightedPick(IReadOnlyList<ResolvedArtifact> artifacts)
    {
        double sum = 0;
        foreach (var a in artifacts) sum += a.EffectiveChance;

        if (sum <= 0) return artifacts[0];

        var r = new Random();
        double roll = r.NextDouble() * sum;
        double acc = 0;

        foreach (var a in artifacts)
        {
            acc += a.EffectiveChance;
            if (roll <= acc) return a;
        }

        return artifacts[^1];
    }
}
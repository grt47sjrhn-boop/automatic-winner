using System;
using substrate_shared.interfaces.core;
using substrate_shared.structs;
using substrate_shared.types.core;

public sealed class CycleRunner
{
    private BiasVector _bias;
    private readonly IResolutionLens _lens;
    private readonly IArtifactFactory _factory;
    private readonly ITraitResolver _resolver;
    private readonly IBiasDriftPolicy _driftPolicy;

    public CycleRunner(
        BiasVector initialBias,
        IResolutionLens lens,
        IArtifactFactory factory,
        ITraitResolver resolver,
        IBiasDriftPolicy driftPolicy)
    {
        _bias = initialBias;
        _lens = lens;
        _factory = factory;
        _resolver = resolver;
        _driftPolicy = driftPolicy;
    }

    public ArtifactCore RunCycle(MoodVector input)
    {
        // Convert input to delta relative to current bias
        var delta = input.ToBiasDelta(_bias);

        // Compute magnitude (L1 or L2 depending on your design)
        float magnitude = input.MagnitudeL1; // or MagnitudeL2

        // Resolve layers from delta + bias + magnitude
        var layers = _lens.Resolve(delta, _bias, magnitude);

        // Create artifact using layers + magnitude
        var artifact = _factory.Create(layers, magnitude);

        // Apply drift and persist updated bias
        _bias = _driftPolicy.Apply(_bias, delta, magnitude, artifact, layers);

        Console.WriteLine($"Delta: {delta.Resonance}, {delta.Warmth}, {delta.Irony}, {delta.Flux}, {delta.Inertia}, {delta.Variance}, {delta.HiddenPressure}");
        Console.WriteLine($"Magnitude: {magnitude}");
        
        return artifact;
    }

    public BiasVector GetBiasSnapshot() => _bias;
}
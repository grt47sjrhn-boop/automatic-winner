using System;
using System.Collections.Generic;
using substrate_shared.Profiles;

namespace substrate_shared.Factories;

public static class DuelistFactory
{
    private static readonly Random _rng = new Random();

    // Create a random duelist with varied bias/resilience
    public static Duelist CreateRandom(string? name = null)
    {
        string generatedName = name ?? $"Opponent_{Guid.NewGuid().ToString()[..6]}";

        // Bias between -1.0 and +1.0
        double bias = _rng.NextDouble() * 2 - 1;

        // Resilience between 0.5 and 1.5
        double resilience = 0.5 + _rng.NextDouble();

        return new Duelist(generatedName, bias)
        {
            // Optionally initialize resilience differently
        };
    }

    // Create a themed duelist (e.g., Joy vs Despair archetypes)
    public static Duelist CreateArchetype(string archetype)
    {
        return archetype.ToLower() switch
        {
            "joy" => new Duelist("Joyful Spirit", initialBias: 0.8),
            "despair" => new Duelist("Shadow of Despair", initialBias: -0.8),
            "neutral" => new Duelist("Equilibrium Seeker", initialBias: 0.0),
            _ => CreateRandom(archetype)
        };
    }

    // Create a batch of random opponents
    public static List<Duelist> CreateBatch(int count)
    {
        var duelists = new List<Duelist>();
        for (int i = 0; i < count; i++)
        {
            duelists.Add(CreateRandom());
        }
        return duelists;
    }
}
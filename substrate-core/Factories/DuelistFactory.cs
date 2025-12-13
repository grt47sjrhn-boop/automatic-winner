using System;
using System.Collections.Generic;
using substrate_core.Models;
using substrate_shared.interfaces.core.Factories;
using substrate_shared.interfaces.Profiles;

namespace substrate_core.Factories
{
    /// <summary>
    /// Concrete factory for creating Duelist instances.
    /// </summary>
    public class DuelistFactory : IDuelistFactory
    {
        private readonly Random _rng = new Random();

        public IDuelist CreateRandom(string? name = null)
        {
            var generatedName = name ?? $"Opponent_{Guid.NewGuid().ToString()[..6]}";

            var bias = _rng.NextDouble() * 2 - 1;       // Bias between -1.0 and +1.0
            var resilience = 0.5 + _rng.NextDouble();   // Resilience between 0.5 and 1.5

            return new Duelist(generatedName, bias);
        }

        public IDuelist CreateArchetype(string archetype)
        {
            return archetype.ToLower() switch
            {
                "joy"      => new Duelist("Joyful Spirit", initialBias: 0.8),
                "despair"  => new Duelist("Shadow of Despair", initialBias: -0.8),
                "neutral"  => new Duelist("Equilibrium Seeker", initialBias: 0.0),
                _          => CreateRandom(archetype)
            };
        }

        public List<IDuelist> CreateBatch(int count)
        {
            var duelists = new List<IDuelist>();
            for (var i = 0; i < count; i++)
            {
                duelists.Add(CreateRandom());
            }
            return duelists;
        }
    }
}
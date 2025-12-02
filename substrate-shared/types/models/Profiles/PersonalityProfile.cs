using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.types.structs;

namespace substrate_shared.types.models.Profiles
{
    /// <summary>
    /// PersonalityProfile aggregates VectorBias summaries and resolver-derived traits/triggers
    /// into a narratable snapshot for contributors.
    /// </summary>
    public class PersonalityProfile
    {
        private VectorBias _bias { get; set; }
        public VectorBias Bias => _bias;

        // Traits/events/intents/tones are now owned here, not on VectorBias
        public List<Trait> Traits { get; set; } = new();
        public List<TriggerEvent> TriggerEvents { get; set; } = new();

        // Resolver summaries (Delta, Persistence, Volatility, Tone, etc.)
        public List<ISummary> Summaries { get; private set; } = new();

        public PersonalityProfile(VectorBias bias)
        {
            UpdateBias(bias);
        }

        public void UpdateBias(VectorBias bias)
        {
            _bias = bias;

            // Collect summaries from bias
            Summaries = bias.Summaries.Values.ToList();
        }

        /// <summary>
        /// Called by PersonalityResolver to inject traits/triggers after bias math is resolved.
        /// </summary>
        public void AddTraits(IEnumerable<Trait> traits)
        {
            Traits.AddRange(traits);
        }

        public void AddTriggers(IEnumerable<TriggerEvent> triggers)
        {
            TriggerEvents.AddRange(triggers);
        }

        /// <summary>
        /// Builds a narratable description of the profile by iterating summaries, traits, and triggers.
        /// </summary>
        public string Describe()
        {
            var lines = new List<string>
            {
                $"[PersonalityProfile] Tick {_bias.TickId}"
            };

            foreach (var summary in Summaries)
            {
                lines.Add("  " + summary.Describe());
            }

            if (Traits.Any())
                lines.Add($"  Traits: {string.Join(", ", Traits.Select(t => t.Label))}");

            if (TriggerEvents.Any())
                lines.Add($"  TriggerEvents: {string.Join(", ", TriggerEvents.Select(e => e.Type))}");

            return string.Join(Environment.NewLine, lines);
        }

        public void Print()
        {
            Console.WriteLine(Describe());
        }
    }
}
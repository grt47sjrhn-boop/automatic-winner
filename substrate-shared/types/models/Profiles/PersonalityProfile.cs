using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.types.structs;
using substrate_shared.enums;

namespace substrate_shared.types.models.Profiles
{
    /// <summary>
    /// PersonalityProfile aggregates VectorBias summaries and resolver-derived traits/triggers
    /// into a narratable snapshot for contributors. It also exposes the resolved personality state
    /// and hardened bias type from PersonalityResolver.
    /// </summary>
    public class PersonalityProfile
    {
        private VectorBias _bias;
        public VectorBias Bias => _bias;

        // Traits/events/intents/tones are now owned here, not on VectorBias
        public List<Trait> Traits { get; private set; } = new();
        public List<TriggerEvent> TriggerEvents { get; private set; } = new();

        // Resolver summaries (Delta, Persistence, Volatility, Tone, etc.)
        public List<ISummary> Summaries { get; private set; } = new();

        // Personality state + hardened bias overlay
        public PersonalityState State { get; private set; } = PersonalityState.Neutral;
        public HardenedBiasType HardenedBias { get; private set; } = HardenedBiasType.None;

        public PersonalityProfile(VectorBias bias)
        {
            UpdateBias(bias);
        }

        public void UpdateBias(VectorBias bias)
        {
            _bias = bias;
            Summaries = bias.Summaries.Values.ToList();
        }

        public void ApplyResolution(ResolutionResult result)
        {
            State = result.PersonalityState;
            HardenedBias = result.HardenedBias;
        }

        public void AddTraits(IEnumerable<Trait> traits)
        {
            Traits.AddRange(traits);
        }

        public void AddTriggers(IEnumerable<TriggerEvent> triggers)
        {
            TriggerEvents.AddRange(triggers);
        }

        /// <summary>
        /// Builds a narratable description of the profile by iterating summaries, traits, triggers,
        /// and resolved personality state.
        /// </summary>
        public string Describe()
        {
            var lines = new List<string>
            {
                $"[PersonalityProfile] Tick {_bias.TickId}, State={State}, HardenedBias={HardenedBias}"
            };

            foreach (var summary in Summaries)
                lines.Add("  " + summary.Describe());

            if (Traits.Any())
                lines.Add($"  Traits: {string.Join(", ", Traits.Select(t => t.Label))}");

            if (TriggerEvents.Any())
                lines.Add($"  TriggerEvents: {string.Join(", ", TriggerEvents.Select(e => $"{e.Type} (score={e.Score:F2})"))}");

            return string.Join(Environment.NewLine, lines);
        }

        public void Print()
        {
            Console.WriteLine(Describe());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models.StateMachines;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_shared.types.models.Profiles
{
    public class PersonalityProfile
    {
        private VectorBias _bias;
        private PersonalityStateMachine _stateMachine;

        public VectorBias Bias => _bias;
        public List<ISummary> Summaries { get; private set; } = [];
        public List<Trait> Traits { get; private set; } = [];
        public List<TriggerEvent> TriggerEvents { get; private set; } = [];

        // Read-only projections from state machine
        public PersonalityState State => _stateMachine.CurrentState;
        public HardenedBiasType HardenedBias => _stateMachine.HardenedBias;

        // Intent from IntentActionSummary
        public IntentType Intent { get; private set; } = IntentType.None;

        public PersonalityProfile(VectorBias bias, PersonalityStateMachine stateMachine)
        {
            _bias = bias;
            _stateMachine = stateMachine;
            UpdateBias(bias);
            
            // Let the profile drive the state machine update
            _stateMachine.Update(this);
        }

        public void UpdateBias(VectorBias bias)
        {
            _bias = bias;
            Summaries = bias.Summaries.Values.ToList();

            // Surface intent if present
            var intentSummary = Summaries.OfType<IntentActionSummary>().FirstOrDefault();
            if (intentSummary != null)
                Intent = intentSummary.Intent;
        }

        public void AddTraits(IEnumerable<Trait> traits) => Traits.AddRange(traits);
        public void AddTriggers(IEnumerable<TriggerEvent> triggers) => TriggerEvents.AddRange(triggers);

        public string Describe()
        {
            var lines = new List<string>
            {
                $"[PersonalityProfile] Tick {_bias.TickId}, State={State}, HardenedBias={HardenedBias}, Intent={Intent}"
            };

            foreach (var summary in Summaries)
                lines.Add("  " + summary.Describe());

            if (Traits.Any())
                lines.Add($"  Traits: {string.Join(", ", Traits.Select(t => t.Label))}");

            if (TriggerEvents.Any())
                lines.Add($"  TriggerEvents: {string.Join(", ", TriggerEvents.Select(e => $"{e.Type} (score={e.Score:F2})"))}");

            return string.Join(Environment.NewLine, lines);
        }
    }
}
using System;
using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Descriptors.Frames;
using substrate_shared.Descriptors.Payloads;
using substrate_shared.Descriptors.Types;
using substrate_shared.Providers.Base;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using substrate_shared.Strategies;

namespace substrate_shared.Providers.Contract.Types
{
    public class SimulationFrameProvider : ISimulationFrameProvider
    {
        private readonly ToneManager _toneManager;

        public SimulationFrameProvider(ToneManager toneManager)
        {
            _toneManager = toneManager;
        }

        public SimulationFrame GetFrame() => CreateEmptyFrame();

        public IEnumerable<IDataFrame> GenerateFrames()
        {
            foreach (IntentAction action in Enum.GetValues(typeof(IntentAction)))
            {
                yield return CreateFrameForIntent(action);
            }
        }

        private static SimulationFrame CreateEmptyFrame() =>
            new SimulationFrame
            {
                Intent = null,
                Context = null,
                Channel = null,
                Subjects = new List<SubjectDescriptor>(),
                Modifiers = new List<ModifierDescriptor>(),
                Triggers = new List<TriggerDescriptor>(),
                Conditions = new List<ConditionDescriptor>(),
                Results = new List<ResultDescriptor>(),
                DescriptorRegistry = new Dictionary<string, BaseDescriptor>(),
                InputPayload = new PayloadMap(),
                OutputPayload = new PayloadMap()
            };

        private SimulationFrame CreateFrameForIntent(IntentAction action)
        {
            var bias   = SuperRegistryManager.GetBias(action);
            var group  = SuperRegistryManager.GetGroup(action);
            var scale  = SuperRegistryManager.GetScaleValue(action);
            var desc   = SuperRegistryManager.GetDescription(action);

            ToneType toneType = action switch
            {
                // Positive supportive
                IntentAction.Comfort   => ToneType.Calm,
                IntentAction.Encourage => ToneType.Resilient,
                IntentAction.Support   => ToneType.Resilient,

                // Positive affirmative
                IntentAction.Inspire   => ToneType.Joy,
                IntentAction.Approve   => ToneType.Radiant,
                IntentAction.Celebrate => ToneType.Radiant,

                // Neutral equilibrium
                IntentAction.Inform    => ToneType.Reflective,
                IntentAction.Question  => ToneType.Equilibrium,
                IntentAction.Reflect   => ToneType.Composite,
                IntentAction.Observe   => ToneType.Equilibrium,

                // Negative hostile (explicit mappings for each)
                IntentAction.Criticize => ToneType.Hostile,
                IntentAction.Challenge => ToneType.Hostile,
                IntentAction.Reject    => ToneType.Hostile,
                IntentAction.Cynical   => ToneType.Despairing,

                // Negative fractured
                IntentAction.Distract  => ToneType.Forsaken,

                // Fallback to bias/group mapping for anything not explicitly listed
                _ => (bias, group) switch
                {
                    (Bias.Positive, NarrativeGroup.SupportiveActions) => ToneType.Resilient,
                    (Bias.Positive, NarrativeGroup.AffirmativeActions) => ToneType.Radiant,
                    (Bias.Neutral, NarrativeGroup.Equilibrium)        => ToneType.Equilibrium,
                    (Bias.Negative, NarrativeGroup.HostileActions)    => ToneType.Hostile,
                    (Bias.Negative, NarrativeGroup.FracturedActions)  => ToneType.Forsaken,
                    (Bias.Negative, NarrativeGroup.AbyssalStates)     => ToneType.Despairing,
                    _                                                 => ToneType.Neutral
                }
            };


            var tone = _toneManager.GetTone(toneType);

            var intent = new IntentDescriptor
            {
                Id = Guid.NewGuid().ToString(),
                IntentType = action,
                Bias = bias,
                Group = group,
                ScaleValue = scale,
                Narrative = desc,
                IntentTone = tone
            };

            // Build strategy directly from descriptor
            var strategy = new IntentActionStrategy(intent);

            return new SimulationFrame
            {
                Intent = intent,
                Context = null,
                Channel = null,
                Subjects = new List<SubjectDescriptor>(),
                Modifiers = new List<ModifierDescriptor>(),
                Triggers = new List<TriggerDescriptor>(),
                Conditions = new List<ConditionDescriptor>(),
                Results = new List<ResultDescriptor>(),
                DescriptorRegistry = new Dictionary<string, BaseDescriptor>
                {
                    { intent.Id, intent }
                },
                InputPayload = new PayloadMap(),
                OutputPayload = new PayloadMap(),
                Strategy = strategy // optional: keep strategy inside frame
            };
        }
    }
}
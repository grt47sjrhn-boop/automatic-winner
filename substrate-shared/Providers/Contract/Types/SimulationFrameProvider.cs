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

// for ToneManager

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
            // Delegate tone construction to ToneManager
            var tone = _toneManager.GetTone(
                action switch
                {
                    IntentAction.Encourage => ToneType.Harmonious,
                    IntentAction.Criticize => ToneType.Critical,
                    IntentAction.Observe   => ToneType.Neutral,
                    _                      => ToneType.Neutral
                }
            );

            var intent = new IntentDescriptor
            {
                Id = Guid.NewGuid().ToString(),
                IntentType = action,
                Tone = tone
            };

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
                OutputPayload = new PayloadMap()
            };
        }
    }
}
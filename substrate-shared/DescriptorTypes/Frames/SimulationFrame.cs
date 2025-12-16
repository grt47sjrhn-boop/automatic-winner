using System.Collections.Generic;
using substrate_shared.DescriptorTypes.Base;
using substrate_shared.DescriptorTypes.Payloads;
using substrate_shared.DescriptorTypes.Types;
using substrate_shared.Providers.Base;

namespace substrate_shared.DescriptorTypes.Frames
{
    public class SimulationFrame : IDataFrame
    {
        public IntentDescriptor? Intent { get; set; }
        public ContextDescriptor? Context { get; set; }
        public ChannelDescriptor? Channel { get; set; }

        public IReadOnlyList<SubjectDescriptor> Subjects { get; set; } 
        public IReadOnlyList<ModifierDescriptor> Modifiers { get; set; } 

        public IReadOnlyList<TriggerDescriptor> Triggers { get; set; } 
        public IReadOnlyList<ConditionDescriptor> Conditions { get; set; } 
        public IReadOnlyList<ResultDescriptor> Results { get; set; }

        // Optional: Registry for resolving descriptor references by ID
        public IReadOnlyDictionary<string, BaseDescriptor> DescriptorRegistry { get; set; }

        // Optional: Runtime payload state (pre- and post-blending)
        public PayloadMap InputPayload { get; set; } = new();
        public PayloadMap OutputPayload { get; set; } = new();
    }
}
using System.Collections.Generic;
using substrate_shared.DescriptorTypes.Base;
using substrate_shared.DescriptorTypes.Payloads;
using substrate_shared.DescriptorTypes.Types;

namespace substrate_shared.Providers.Base
{
    public interface IDataFrame
    {
        IntentDescriptor? Intent { get; }
        ContextDescriptor? Context { get; }
        ChannelDescriptor? Channel { get; }

        IReadOnlyList<SubjectDescriptor> Subjects { get; }
        IReadOnlyList<ModifierDescriptor> Modifiers { get; }
        IReadOnlyList<TriggerDescriptor> Triggers { get; }
        IReadOnlyList<ConditionDescriptor> Conditions { get; }
        IReadOnlyList<ResultDescriptor> Results { get; }

        IReadOnlyDictionary<string, BaseDescriptor> DescriptorRegistry { get; }

        PayloadMap InputPayload { get; }
        PayloadMap OutputPayload { get; }
    }
}
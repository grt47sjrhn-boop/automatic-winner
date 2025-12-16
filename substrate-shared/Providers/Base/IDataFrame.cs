using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Payloads;
using substrate_shared.Descriptors.Types;

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
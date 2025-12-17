using substrate_shared.Descriptors.Enums;
using substrate_shared.Descriptors.Types;
using substrate_shared.Registries.Base;

namespace substrate_shared.Strategies
{
    public sealed class IntentActionStrategy
    {
        public string ActionId { get; }
        public DescriptorType Type { get; }
        public NarrativeTone? Tone { get; }
        public string? Narrative { get; }


        public IntentActionStrategy(string actionId, DescriptorType type, NarrativeTone? tone = null)
        {
            ActionId = actionId;
            Type = type;
            Tone = tone;
        }
        
        // New overload: build directly from descriptor
        public IntentActionStrategy(IntentDescriptor intent)
        {
            ActionId = intent.Id;
            Type = intent.Type;
            Tone = intent.IntentTone;
            Narrative = intent.Narrative;
        }
    }
}
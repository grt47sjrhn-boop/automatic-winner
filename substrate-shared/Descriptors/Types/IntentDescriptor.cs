using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;

namespace substrate_shared.Descriptors.Types
{
    public class IntentDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Intent;

        public IntentAction IntentType { get; set; }
        public Bias Bias { get; set; }
        public NarrativeGroup Group { get; set; }
        public int ScaleValue { get; set; }

        // Narrative text from RegistryNarrative attribute
        public string? Narrative { get; set; }

        // Tone assigned by ToneManager
        public NarrativeTone? IntentTone { get; set; }

        // Optional metadata
        public string? TargetId { get; set; }
        public List<string>? Tags { get; set; }
        public float? Confidence { get; set; }

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();

            dict["intentType"] = IntentType.ToString();
            dict["bias"] = Bias.ToString();
            dict["group"] = Group.ToString();
            dict["scaleValue"] = ScaleValue;

            if (!string.IsNullOrWhiteSpace(Narrative))
                dict["narrative"] = Narrative;

            if (IntentTone != null)
                dict["intentTone"] = IntentTone.ToString();

            if (!string.IsNullOrWhiteSpace(TargetId))
                dict["target"] = TargetId;

            if (Tags is { Count: > 0 })
                dict["tags"] = Tags;

            if (Confidence != null)
                dict["confidence"] = Confidence;

            return dict;
        }
    }
}
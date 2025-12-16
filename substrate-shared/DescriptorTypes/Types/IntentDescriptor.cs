using System.Collections.Generic;
using substrate_shared.DescriptorTypes.Base;
using substrate_shared.DescriptorTypes.Enums;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;

namespace substrate_shared.DescriptorTypes.Types
{
    public class IntentDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Intent;

        public IntentAction IntentType { get; set; }
        public Bias Bias { get; set; }
        public NarrativeGroup Group { get; set; }
        public int ScaleValue { get; set; }

        public string? Narrative { get; set; }
        public NarrativeTone? IntentTone { get; set; }

        public string? TargetId { get; set; }              // Optional: target subject
        public List<string>? Tags { get; set; }            // Optional: for filtering or grouping
        public float? Confidence { get; set; }             // Optional: 0.0–1.0 certainty score

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

            if (Tags != null && Tags.Count > 0)
                dict["tags"] = Tags;

            if (Confidence != null)
                dict["confidence"] = Confidence;

            return dict;
        }
    }
}
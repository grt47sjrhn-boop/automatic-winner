using System.Collections.Generic;
using substrate_shared.DescriptorTypes.Base;
using substrate_shared.DescriptorTypes.Enums;

namespace substrate_shared.DescriptorTypes.Types
{
    public class ContextDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Context;

        public string? Mood { get; set; }
        public string? Setting { get; set; }
        public int Urgency { get; set; }
        public string? NarrativeBackdrop { get; set; }

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();

            if (!string.IsNullOrWhiteSpace(Mood))
                dict["mood"] = Mood;

            if (!string.IsNullOrWhiteSpace(Setting))
                dict["setting"] = Setting;

            if (Urgency != 0)
                dict["urgency"] = Urgency;

            if (!string.IsNullOrWhiteSpace(NarrativeBackdrop))
                dict["narrativeBackdrop"] = NarrativeBackdrop;

            return dict;
        }
    }
}
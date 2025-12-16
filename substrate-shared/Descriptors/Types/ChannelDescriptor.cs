using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Enums;

namespace substrate_shared.Descriptors.Types
{
    public class ChannelDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Channel;

        public string Medium { get; set; } // e.g., "dialogue", "broadcast", "dream", "ritual"
        public string? Style { get; set; } // e.g., "formal", "cryptic", "sarcastic"
        public int Clarity { get; set; }   // e.g., 0–10 scale

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();
            dict["medium"] = Medium;
            dict["clarity"] = Clarity;
            if (!string.IsNullOrWhiteSpace(Style))
                dict["style"] = Style;
            return dict;
        }
    }
}
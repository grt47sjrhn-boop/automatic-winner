using System.Collections.Generic;
using substrate_shared.DescriptorTypes.Base;
using substrate_shared.DescriptorTypes.Enums;
using substrate_shared.Registries.enums;

namespace substrate_shared.DescriptorTypes.Types
{
    public class SubjectDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Subject;

        public string SubjectId { get; set; } // e.g., "npc:merchant:elira"
        public string? Role { get; set; }     // e.g., "antagonist", "ally", "observer"
        public string? Archetype { get; set; } // e.g., healer, trader, merchant
        public Bias Bias { get; set; }

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();
            dict["subjectId"] = SubjectId;
            dict["bias"] = Bias.ToString();
            if (!string.IsNullOrWhiteSpace(Role))
                dict["role"] = Role;
            return dict;
        }
    }
}
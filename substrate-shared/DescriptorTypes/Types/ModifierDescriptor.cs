using System.Collections.Generic;
using substrate_shared.DescriptorTypes.Base;
using substrate_shared.DescriptorTypes.Enums;
using substrate_shared.Registries.enums;

namespace substrate_shared.DescriptorTypes.Types
{
    public class ModifierDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Modifier;

        public DescriptorType ModifierTarget { get; set; } // e.g., Intent, Subject, Channel
        public Bias Bias { get; set; }
        public int Intensity { get; set; }
        public string? EffectDescription { get; set; }

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();
            dict["modifierTarget"] = ModifierTarget.ToString();
            dict["bias"] = Bias.ToString();
            dict["intensity"] = Intensity;
            if (!string.IsNullOrWhiteSpace(EffectDescription))
                dict["effectDescription"] = EffectDescription;
            return dict;
        }
    }
}
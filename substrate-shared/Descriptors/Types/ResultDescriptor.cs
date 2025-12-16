using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Registries.enums;

namespace substrate_shared.Descriptors.Types
{
    public class ResultDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Result;
        public string? ResolutionLabel { get; set; }
        public ModifierDescriptor ResultFormula { get; set; }
        public Bias Bias { get; set; }

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();
            dict["resultFormula"] = ResultFormula.Id;
            dict["bias"] = Bias.ToString();
            return dict;
        }
    }
}
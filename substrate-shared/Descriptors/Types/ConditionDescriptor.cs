using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Enums;

namespace substrate_shared.Descriptors.Types
{
    public class ConditionDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Condition;

        public string LeftOperand { get; set; }               // e.g., "trust:subject"
        public ConditionOperator Operator { get; set; }       // e.g., LessThan
        public string RightOperand { get; set; }              // e.g., "-3"

        public ConditionLogic Logic { get; set; } = ConditionLogic.And;
        public bool Negate { get; set; } = false;

        public ConditionScope Scope { get; set; } = ConditionScope.Self; // NEW
        public string? ConditionGroupId { get; set; }                    // NEW

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();

            dict["left"] = LeftOperand;
            dict["operator"] = Operator.ToString();
            dict["right"] = RightOperand;
            dict["logic"] = Logic.ToString();
            dict["scope"] = Scope.ToString();

            if (Negate)
                dict["negate"] = true;

            if (!string.IsNullOrWhiteSpace(ConditionGroupId))
                dict["group"] = ConditionGroupId;

            return dict;
        }
    }
}
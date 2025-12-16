using System;
using substrate_shared.DescriptorTypes.Enums;
using substrate_shared.DescriptorTypes.Interfaces;
using substrate_shared.DescriptorTypes.Types;

namespace substrate_shared.DescriptorTypes.Validators;

public class ConditionDescriptorValidator : IDescriptorValidator<ConditionDescriptor>
{
    public bool IsValid(ConditionDescriptor descriptor, out string? error)
    {
        if (string.IsNullOrWhiteSpace(descriptor.LeftOperand) || string.IsNullOrWhiteSpace(descriptor.RightOperand))
        {
            error = "Condition must have both Left and Right operands.";
            return false;
        }

        if (!Enum.IsDefined(typeof(ConditionOperator), descriptor.Operator))
        {
            error = $"Invalid Operator: {descriptor.Operator}";
            return false;
        }

        error = null;
        return true;
    }
}
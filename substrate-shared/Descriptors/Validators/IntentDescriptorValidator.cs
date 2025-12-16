using System;
using substrate_shared.Descriptors.Interfaces;
using substrate_shared.Descriptors.Types;
using substrate_shared.Registries.enums;

namespace substrate_shared.Descriptors.Validators;

public class IntentDescriptorValidator : IDescriptorValidator<IntentDescriptor>
{
    public bool IsValid(IntentDescriptor descriptor, out string? error)
    {
        if (!Enum.IsDefined(typeof(IntentAction), descriptor.IntentType))
        {
            error = $"Invalid IntentType: {descriptor.IntentType}";
            return false;
        }

        if (!Enum.IsDefined(typeof(Bias), descriptor.Bias))
        {
            error = $"Invalid Bias: {descriptor.Bias}";
            return false;
        }

        if (!Enum.IsDefined(typeof(NarrativeGroup), descriptor.Group))
        {
            error = $"Invalid NarrativeGroup: {descriptor.Group}";
            return false;
        }

        if (descriptor.ScaleValue < 0)
        {
            error = "ScaleValue must be non-negative.";
            return false;
        }

        error = null;
        return true;
    }
}
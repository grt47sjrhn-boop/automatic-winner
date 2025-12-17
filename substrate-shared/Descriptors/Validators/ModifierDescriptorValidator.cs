using System;
using substrate_shared.Descriptors.Interfaces;
using substrate_shared.Descriptors.Types;
using substrate_shared.Registries.enums;

namespace substrate_shared.Descriptors.Validators
{
    public class ModifierDescriptorValidator : IDescriptorValidator<ModifierDescriptor>
    {
        public bool IsValid(ModifierDescriptor descriptor, out string? error)
        {
            if (descriptor.Intensity < 0)
            {
                error = "Modifier intensity must be non-negative.";
                return false;
            }

            if (!Enum.IsDefined(typeof(Bias), descriptor.Bias))
            {
                error = $"Invalid Bias: {descriptor.Bias}";
                return false;
            }

            if (string.IsNullOrWhiteSpace(descriptor.ModifierTarget.ToString()))
            {
                error = "ModifierTarget is required.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
using System;
using substrate_shared.Descriptors.Interfaces;
using substrate_shared.Descriptors.Types;
using substrate_shared.Registries.enums;

namespace substrate_shared.Descriptors.Validators
{
    public class ResultDescriptorValidator : IDescriptorValidator<ResultDescriptor>
    {
        public bool IsValid(ResultDescriptor descriptor, out string? error)
        {
            if (!Enum.IsDefined(typeof(Bias), descriptor.Bias))
            {
                error = $"Invalid Bias: {descriptor.Bias}";
                return false;
            }

            if (string.IsNullOrWhiteSpace(descriptor.Id))
            {
                error = "ResultDescriptor must have a valid Id.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
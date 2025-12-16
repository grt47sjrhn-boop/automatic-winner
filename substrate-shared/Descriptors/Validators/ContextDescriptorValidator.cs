using substrate_shared.Descriptors.Interfaces;
using substrate_shared.Descriptors.Types;

namespace substrate_shared.Descriptors.Validators;

public class ContextDescriptorValidator : IDescriptorValidator<ContextDescriptor>
{
    public bool IsValid(ContextDescriptor descriptor, out string? error)
    {
        if (descriptor.Urgency < 0)
        {
            error = "Urgency must be non-negative.";
            return false;
        }

        error = null;
        return true;
    }
}
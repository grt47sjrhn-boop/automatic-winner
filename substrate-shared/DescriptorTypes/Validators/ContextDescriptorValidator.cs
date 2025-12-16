using substrate_shared.DescriptorTypes.Interfaces;
using substrate_shared.DescriptorTypes.Types;

namespace substrate_shared.DescriptorTypes.Validators;

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
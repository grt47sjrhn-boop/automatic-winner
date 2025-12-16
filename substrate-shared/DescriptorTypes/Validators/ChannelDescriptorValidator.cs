using substrate_shared.DescriptorTypes.Interfaces;
using substrate_shared.DescriptorTypes.Types;

namespace substrate_shared.DescriptorTypes.Validators;

public class ChannelDescriptorValidator : IDescriptorValidator<ChannelDescriptor>
{
    public bool IsValid(ChannelDescriptor descriptor, out string? error)
    {
        if (string.IsNullOrWhiteSpace(descriptor.Medium))
        {
            error = "Channel must specify a Medium.";
            return false;
        }

        error = null;
        return true;
    }
}
using substrate_shared.Descriptors.Interfaces;
using substrate_shared.Descriptors.Types;

namespace substrate_shared.Descriptors.Validators
{
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
}
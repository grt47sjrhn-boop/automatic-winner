using substrate_shared.DescriptorTypes.Interfaces;
using substrate_shared.DescriptorTypes.Types;

namespace substrate_shared.DescriptorTypes.Validators;

public class SubjectDescriptorValidator : IDescriptorValidator<SubjectDescriptor>
{
    public bool IsValid(SubjectDescriptor descriptor, out string? error)
    {
        if (string.IsNullOrWhiteSpace(descriptor.SubjectId))
        {
            error = "SubjectId is required.";
            return false;
        }

        error = null;
        return true;
    }
}
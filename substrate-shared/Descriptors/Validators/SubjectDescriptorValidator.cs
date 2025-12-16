using substrate_shared.Descriptors.Interfaces;
using substrate_shared.Descriptors.Types;

namespace substrate_shared.Descriptors.Validators;

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
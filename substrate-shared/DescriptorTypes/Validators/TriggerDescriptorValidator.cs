using substrate_shared.DescriptorTypes.Interfaces;
using substrate_shared.DescriptorTypes.Types;

namespace substrate_shared.DescriptorTypes.Validators;

public class TriggerDescriptorValidator : IDescriptorValidator<TriggerDescriptor>
{
    public bool IsValid(TriggerDescriptor descriptor, out string? error)
    {
        if (descriptor.Subjects == null || descriptor.Subjects.Count == 0)
        {
            error = "Trigger must reference at least one Subject.";
            return false;
        }

        if (descriptor.Results == null || descriptor.Results.Count == 0)
        {
            error = "Trigger must reference at least one Result.";
            return false;
        }

        error = null;
        return true;
    }
}
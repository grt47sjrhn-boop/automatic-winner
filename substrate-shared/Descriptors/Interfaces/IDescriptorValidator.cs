using substrate_shared.Descriptors.Base;

namespace substrate_shared.Descriptors.Interfaces
{
    public interface IDescriptorValidator<in T> where T : BaseDescriptor
    {
        bool IsValid(T descriptor, out string? error);
    }
}
using substrate_shared.DescriptorTypes.Base;

namespace substrate_shared.DescriptorTypes.Interfaces
{
    public interface IDescriptorValidator<in T> where T : BaseDescriptor
    {
        bool IsValid(T descriptor, out string? error);
    }
}
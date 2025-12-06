

namespace substrate_shared.interfaces
{
    public interface IResolver
    {
        string Name { get; }
        ISummary Resolve();
        void Describe();
    }
}
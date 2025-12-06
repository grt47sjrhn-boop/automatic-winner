using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.interfaces
{
    public interface ISummary
    {
        string Title { get; }
        string Description { get; }
        void Print();
        string ToString();

        // New structured fields
        BiasVector? ResolvedVector { get; }
        DuelOutcome Outcome { get; }
    }
}
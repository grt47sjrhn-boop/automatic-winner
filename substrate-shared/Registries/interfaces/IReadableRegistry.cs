using substrate_shared.Registries.enums;

namespace substrate_shared.Registries.interfaces
{
    public interface IReadableRegistry
    {
        string GetDescription();
        Bias GetBias();
        NarrativeGroup GetGroup();
    }
}
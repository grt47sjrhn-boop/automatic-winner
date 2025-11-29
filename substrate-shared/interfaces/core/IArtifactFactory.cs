using substrate_shared.structs;

namespace substrate_shared.interfaces.core
{
    public interface IArtifactFactory
    {
        ArtifactCore Create(in ResolutionLayers layers, float magnitude);
    }
}
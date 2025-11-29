using substrate_shared.structs;
using substrate_shared.types.core;

namespace substrate_shared.interfaces.extensions
{
    public interface IArtifactHook
    {
        void OnArtifactResolved(Artifact toArtifact, BiasVector ctxNewBias);
    }
}
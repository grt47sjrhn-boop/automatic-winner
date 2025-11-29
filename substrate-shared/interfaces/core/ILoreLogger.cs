using substrate_shared.structs;
using substrate_shared.types.core;
using ArtifactCore = substrate_shared.structs.ArtifactCore;
using ResolutionLayers = substrate_shared.structs.ResolutionLayers;

namespace substrate_shared.interfaces.core
{
    public interface ILoreLogger
    {
        void LogCycle(in MoodVector input, in BiasDelta delta, float magnitude, in ResolutionLayers layers, in ArtifactCore artifact, in Trait trait);
    }
}
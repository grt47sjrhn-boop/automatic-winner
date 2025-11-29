using substrate_shared.enums;

namespace substrate_shared.structs
{
    public readonly struct ArtifactCore
    {
        public readonly ArtifactType Type;
        public ArtifactCore(ArtifactType type) { Type = type; }
    }
}
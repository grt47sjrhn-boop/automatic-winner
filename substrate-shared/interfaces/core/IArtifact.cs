using substrate_shared.enums;
using substrate_shared.structs;

namespace substrate_shared.interfaces.core
{
    public interface IArtifact
    {
        string Id { get; }
        ArtifactType Type { get; }
        double BaseChance { get; }
        double Modifier { get; set; }
        string Outcome { get; set; }
        Trait Trait { get; set; }
    }
}
using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.structs;

namespace substrate_shared.types.factory
{
    public static class ArtifactFactory
    {
        public static List<ResolvedArtifact> DefaultCandidates(Trait trait)
        {
            return new List<ResolvedArtifact>
            {
                new ResolvedArtifact(ArtifactType.Pearl, trait, baseChance: 0.60),
                new ResolvedArtifact(ArtifactType.RarePearl, trait, baseChance: 0.25),
                new ResolvedArtifact(ArtifactType.ChaosCrystal, trait, baseChance: 0.10),
                new ResolvedArtifact(ArtifactType.CatastrophicCollapse, trait, baseChance: 0.05)
            };
        }
    }
}
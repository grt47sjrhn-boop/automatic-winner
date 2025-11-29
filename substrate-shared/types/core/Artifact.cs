using System;
using substrate_shared.enums;
using substrate_shared.interfaces.core;
using substrate_shared.structs;

namespace substrate_shared.types.core
{
    public class Artifact : IArtifact
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public ArtifactType Type { get; private set; }
        public double BaseChance { get; private set; }
        public double Modifier { get; set; }
        public string Outcome { get; set; }
        public Trait Trait { get; set; }

        public Artifact(ArtifactType type, double baseChance, Trait trait)
        {
            Type = type;
            BaseChance = baseChance;
            Modifier = 0.0;
            Trait = trait;
        }
    }
}
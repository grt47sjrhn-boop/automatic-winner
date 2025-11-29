using substrate_shared.enums;
using substrate_shared.interfaces.core;
using substrate_shared.structs;

namespace substrate_shared.Defaults
{
    public sealed class DefaultArtifactFactory : IArtifactFactory
    {
        public ArtifactCore Create(in ResolutionLayers layers, float magnitude)
        {
            if (magnitude <= 1e-6f)
                return new ArtifactCore(ArtifactType.SilentCycle);

            if (layers.Hidden > 0.3f && magnitude > 1.0f)
                return new ArtifactCore(ArtifactType.ChaosCrystal);

            if (magnitude > 3.0f)
                return new ArtifactCore(ArtifactType.CatastrophicCollapse);

            if (magnitude > 1.0f)
                return new ArtifactCore(ArtifactType.RarePearl);

            return new ArtifactCore(ArtifactType.Pearl);
        }

        #region Summary
        // Signature: DefaultArtifactFactory.Create(ResolutionLayers layers, float magnitude) -> ArtifactCore
        // PRISMx Version: 1.1 | Date: 2025.111.28
        // Tags: #substrate #dll #unity
        #endregion
    }
}
using System;
using System.Collections.Generic;
using substrate_shared.Records.Codex;

namespace substrate_shared.interfaces.Codex
{
    public interface IProfileRegistry
    {
        // Registers a new contributor profile with presets and metadata
        void RegisterProfile(Profile profile);

        // Retrieves a profile by contributor ID
        Profile GetProfile(Guid contributorId);

        // Returns ancestry lineage for a given profile
        Lineage GetLineage(Guid contributorId);

        // Updates presets or narrative flavor for a profile
        void UpdateProfile(Guid contributorId, Preset preset);

        // Lists all active profiles (for contributor portals)
        IEnumerable<Profile> ListProfiles();

        // Provides ancestry metadata for engine requests
        AncestryMetadata GetAncestryMetadata(Guid contributorId);
    }
}
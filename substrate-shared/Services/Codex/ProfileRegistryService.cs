using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Records.Codex;
using substrate_shared.interfaces.Codex;

namespace substrate_shared.Services.Codex
{
    /// <summary>
    /// In-memory implementation of IProfileRegistry.
    /// Tracks contributor profiles, presets, and ancestry lineage.
    /// </summary>
    public sealed class ProfileRegistryService : IProfileRegistry
    {
        private readonly Dictionary<Guid, Profile> _profiles = new();
        private readonly Dictionary<Guid, Lineage> _lineages = new();

        public void RegisterProfile(Profile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            _profiles[profile.ContributorId] = profile;

            // initialize lineage if not present
            if (!_lineages.ContainsKey(profile.ContributorId))
            {
                _lineages[profile.ContributorId] = new Lineage(
                    profile.ContributorId,
                    [],
                    profile.Preset.Difficulty
                );
            }
        }

        public Profile GetProfile(Guid contributorId)
        {
            _profiles.TryGetValue(contributorId, out var profile);
            return profile;
        }

        public Lineage GetLineage(Guid contributorId)
        {
            _lineages.TryGetValue(contributorId, out var lineage);
            return lineage;
        }

        public void UpdateProfile(Guid contributorId, Preset preset)
        {
            if (_profiles.TryGetValue(contributorId, out var profile))
            {
                // records are immutable, so create a new one
                var updated = profile with { Preset = preset };
                _profiles[contributorId] = updated;
            }
        }

        public IEnumerable<Profile> ListProfiles() => _profiles.Values;

        public AncestryMetadata GetAncestryMetadata(Guid contributorId)
        {
            var profile = GetProfile(contributorId);
            var lineage = GetLineage(contributorId);

            if (profile == null || lineage == null) return null;

            return new AncestryMetadata(
                contributorId,
                lineage,
                profile.Preset,
                profile.Flavor
            );
        }

        /// <summary>
        /// Initialize the registry (clear any existing state).
        /// </summary>
        public void Initialize()
        {
            _profiles.Clear();
            _lineages.Clear();
        }

        /// <summary>
        /// Shutdown the registry (clear state and release resources).
        /// </summary>
        public void Shutdown()
        {
            _profiles.Clear();
            _lineages.Clear();
        }

        /// <summary>
        /// Dispose/Reset the registry (alias for Shutdown).
        /// </summary>
        public void Dispose()
        {
            Shutdown();
        }
    }
}
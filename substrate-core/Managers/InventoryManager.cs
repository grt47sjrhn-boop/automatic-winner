using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Factories;
using substrate_core.Models;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Results;
using substrate_shared.Traits.Base;

namespace substrate_core.Managers
{
    /// <summary>
    /// Manager responsible for storing and retrieving crystals and engagement results.
    /// Implements IManager for orchestration consistency.
    /// </summary>
    public class InventoryManager : IManager
    {
        private readonly List<TraitCrystal> _crystals = new();
        private readonly Dictionary<Guid, EngagementResult> _results = new();

        /// <summary>
        /// Add a forged crystal to inventory.
        /// Automatically assigns ToneCut and RarityTier metadata.
        /// </summary>
        public void AddCrystal(TraitCrystal crystal)
        {
            // 🔹 Create ToneCut from facets
            var toneCut = ResultFactory.CreateToneCut(crystal.Facets);

            // 🔹 Create RarityTier from modifier value
            var rarityTier = ResultFactory.CreateRarityTier(crystal);

            // 🔹 Attach metadata (requires TraitCrystal extension with optional properties)
            crystal.ToneCut = toneCut;
            crystal.RarityTier = rarityTier;

            _crystals.Add(crystal);
        }

        /// <summary>
        /// Add an engagement result to inventory.
        /// </summary>
        public void AddResult(EngagementResult result)
        {
            _results[result.EngagementId] = result;
        }

        /// <summary>
        /// Retrieve a crystal associated with a given engagement or bias seed ID.
        /// </summary>
        public TraitCrystal? GetCrystal(Guid engagementId)
        {
            return _crystals.FirstOrDefault(c => c.EngagementId == engagementId);
        }

        /// <summary>
        /// Get all crystals currently in inventory.
        /// </summary>
        public IReadOnlyList<TraitCrystal> GetCrystals() => _crystals;

        /// <summary>
        /// Get all crystals filtered by bias classification.
        /// </summary>
        public IReadOnlyList<TraitCrystal> GetCrystalsByBias(Bias bias)
        {
            return _crystals.Where(c => c.GetBias() == bias).ToList();
        }

        /// <summary>
        /// Get all crystals filtered by rarity tier.
        /// </summary>
        public IReadOnlyList<TraitCrystal> GetCrystalsByTier(string tier)
        {
            return _crystals.Where(c => c.Rarity.ToString() == tier).ToList();
        }

        /// <summary>
        /// Retrieve the ToneCut for a crystal tied to a given engagement ID.
        /// </summary>
        public IToneCut? GetToneCut(Guid engagementId)
        {
            var crystal = GetCrystal(engagementId);
            return crystal?.ToneCut;
        }

        /// <summary>
        /// Get all engagement results currently stored.
        /// </summary>
        public IReadOnlyDictionary<Guid, EngagementResult> GetResults() => _results;

        /// <summary>
        /// Retrieve a specific engagement result by ID.
        /// </summary>
        public EngagementResult? GetResult(Guid engagementId)
        {
            return _results.GetValueOrDefault(engagementId);
        }

        /// <summary>
        /// Clear inventory (both crystals and results).
        /// </summary>
        public void Clear()
        {
            _crystals.Clear();
            _results.Clear();
        }
        
        /// <summary>
        /// Reset session state: wipe inventory and reset rare-roll factory counters.
        /// </summary>
        public void ResetSession()
        {
            _crystals.Clear();
            _results.Clear();
            TraitCrystalFactory.ResetSession();
        }

    }
}
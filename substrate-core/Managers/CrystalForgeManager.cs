using System.Collections.Generic;
using substrate_core.Summaries.Types;
using substrate_shared.Factories;
using substrate_shared.interfaces;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;
using substrate_core.Engagements.Results;
using substrate_shared.Managers;

namespace substrate_core.Managers
{
    /// <summary>
    /// Manager responsible for forging crystals from engagement results.
    /// Implements IManager for consistency across orchestration.
    /// </summary>
    public class CrystalForgeManager : IManager
    {
        private readonly InventoryManager _inventory;

        public CrystalForgeManager(InventoryManager inventory)
        {
            _inventory = inventory;
        }

        /// <summary>
        /// Forge a single crystal and add it to inventory.
        /// Automatically generates ToneCut and RarityTier.
        /// </summary>
        public TraitCrystal ForgeCrystal(
            int threshold,
            bool isPositive,
            IReadOnlyDictionary<ToneType,int> facets,
            string narrative,
            List<TraitCrystal> existingCrystals)
        {
            var toneCut = ResultFactory.CreateToneCut(facets);
            var rarityTier = ResultFactory.CreateRarityTier(threshold, facets);

            var crystal = TraitCrystalFactory.CreateCrystal(
                threshold,
                isPositive,
                facets,
                narrative,
                existingCrystals,
                toneCut,
                rarityTier
            );

            _inventory.AddCrystal(crystal);
            return crystal;
        }

        /// <summary>
        /// Forge multiple crystals from batch inputs.
        /// </summary>
        public IEnumerable<TraitCrystal> ForgeCluster(
            IEnumerable<(int threshold, bool isPositive, IReadOnlyDictionary<ToneType,int> facets, string narrative)> inputs,
            List<TraitCrystal> existingCrystals)
        {
            var forged = new List<TraitCrystal>();
            foreach (var input in inputs)
            {
                var toneCut = ResultFactory.CreateToneCut(input.facets);
                var rarityTier = ResultFactory.CreateRarityTier(input.threshold, input.facets);

                forged.Add(TraitCrystalFactory.CreateCrystal(
                    input.threshold,
                    input.isPositive,
                    input.facets,
                    input.narrative,
                    existingCrystals,
                    toneCut,
                    rarityTier
                ));
            }

            foreach (var crystal in forged)
                _inventory.AddCrystal(crystal);

            return forged;
        }

        /// <summary>
        /// Generate a summary report of all crystals in inventory.
        /// </summary>
        public TraitCrystalSummary SummarizeInventory(IEnumerable<TraitCrystal> crystals)
        {
            return new TraitCrystalSummary(crystals);
        }
    }
}
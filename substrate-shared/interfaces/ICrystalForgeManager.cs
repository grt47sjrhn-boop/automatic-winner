using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.Traits.Base;

namespace substrate_shared.interfaces
{
    public interface ICrystalForgeManager : IManager
    {
        /// <summary>
        /// Forge a single crystal and add it to inventory.
        /// Automatically generates ToneCut and RarityTier.
        /// </summary>
        TraitCrystal ForgeCrystal(
            int threshold,
            bool isPositive,
            IReadOnlyDictionary<ToneType,int> facets,
            string narrative,
            List<TraitCrystal> existingCrystals
        );

        /// <summary>
        /// Forge multiple crystals from batch inputs.
        /// </summary>
        IEnumerable<TraitCrystal> ForgeCluster(
            IEnumerable<(int threshold, bool isPositive, IReadOnlyDictionary<ToneType,int> facets, string narrative)> inputs,
            List<TraitCrystal> existingCrystals
        );

        /// <summary>
        /// Generate a summary report of all crystals in inventory.
        /// </summary>
        ISummary SummarizeInventory(IEnumerable<TraitCrystal> crystals);
    }
}
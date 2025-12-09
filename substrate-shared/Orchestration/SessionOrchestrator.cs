using System.Collections.Generic;
using substrate_shared.Engagements.Types;
using substrate_shared.Reports;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.structs;

namespace substrate_shared.Orchestration
{
    /// <summary>
    /// Orchestrates a full session: reset, run duels, and build report.
    /// </summary>
    public class SessionOrchestrator
    {
        private readonly InventoryManager _inventory;
        private readonly IResilienceTracker _tracker;
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public SessionOrchestrator(
            InventoryManager inventory,
            IResilienceTracker tracker,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager)
        {
            _inventory     = inventory;
            _tracker       = tracker;
            _biasManager   = biasManager;
            _facetManager  = facetManager;
            _toneManager   = toneManager;
            _rarityManager = rarityManager;
        }

        /// <summary>
        /// Run a full session: reset, execute duels, and produce report.
        /// </summary>
        public ResilienceReport RunSession(IEnumerable<(BiasVector A, BiasVector B)> duelPairs)
        {
            // 🔹 Reset inventory and factory state
            _inventory.ResetSession();

            // 🔹 Run duels
            foreach (var (duelistA, duelistB) in duelPairs)
            {
                var duel = new DuelEngagement(
                    _inventory,
                    duelistA,
                    duelistB,
                    _biasManager,
                    _facetManager,
                    _toneManager,
                    _rarityManager
                );

                duel.Resolve(); // run engagement
                var summary = duel.Finalize();

                // 🔹 Use tracker’s AddSummary method
                _tracker.AddSummary(summary);

                // If you want to also push crystals into tracker explicitly:
                foreach (var crystal in _inventory.GetCrystals())
                {
                    _tracker.AddCrystal(crystal);
                }
            }

            // 🔹 Build report
            var builder = new ReportBuilder(_tracker, _inventory);
            return builder.BuildReport();
        }
    }
}
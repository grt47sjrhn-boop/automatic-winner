using System.Collections.Generic;
using substrate_core.Managers;
using substrate_core.Models.Engagements.Enums;
using substrate_core.Models.Reports;
using substrate_core.Runners.Factories;
using substrate_shared.interfaces;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Managers;
using substrate_shared.Reports;
using substrate_shared.structs;
// 🔹 bring in RunnerFactory
// 🔹 EngagementType enum

namespace substrate_core.Orchestration
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
        public IResilienceReport RunSession(IEnumerable<(BiasVector A, BiasVector B)> duelPairs)
        {
            // 🔹 Reset inventory and factory state
            _inventory.ResetSession();

            // 🔹 Run duels via RunnerFactory
            foreach (var (duelistA, duelistB) in duelPairs)
            {
                var runner = RunnerFactory.Create(
                    EngagementType.Duel,
                    _inventory,
                    _biasManager,
                    _facetManager,
                    _toneManager,
                    _rarityManager,
                    biasSeedId: null,
                    participants: [duelistA, duelistB]
                );

                runner.Run();
                var summary = runner.Engagement.Finalize();

                _tracker.AddSummary(summary);

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
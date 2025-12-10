using System;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Mappers;
using substrate_shared.Registries.enums;
using substrate_shared.Results;
using substrate_shared.structs;
using substrate_shared.Summaries.Types;
using substrate_shared.Traits.Base;

namespace substrate_shared.Runners
{
    /// <summary>
    /// Generic runner that can execute any engagement type.
    /// Centralizes the pipeline: resolve → finalize → result → inventory → forge → composite summary.
    /// </summary>
    public class EngagementRunner : IRunner
    {
        public IEngagement Engagement { get; }
        private readonly InventoryManager _inventory;
        private readonly CrystalForgeManager _forgeManager;

        public EngagementRunner(InventoryManager inventory, IEngagement engagement)
        {
            Engagement = engagement;
            _inventory = inventory;
            _forgeManager = new CrystalForgeManager(inventory);
        }

        public ISummary Run(int ticks = 1)
        {
            Engagement.ResolveStep(ticks);

            var engagementSummary = Engagement.Finalize();

            var result = new EngagementResult
            {
                EngagementId = Guid.NewGuid(),
                Threshold = Engagement.Shape.Values.Count,
                Bias = new BiasDescriptor
                {
                    Bias = engagementSummary.Outcome switch
                    {
                        DuelOutcome.Recovery   => substrate_shared.Registries.enums.Bias.Positive,
                        DuelOutcome.Collapse   => substrate_shared.Registries.enums.Bias.Negative,
                        DuelOutcome.Stalemate  => substrate_shared.Registries.enums.Bias.Neutral,
                        DuelOutcome.Unresolved => substrate_shared.Registries.enums.Bias.Mixed,
                        _                      => substrate_shared.Registries.enums.Bias.Mixed
                    },
                    Narrative = engagementSummary.Description
                },
                Shape = Engagement.Shape,
                Tones = new Dictionary<ToneType,int>(FacetToneMapper.ToToneDictionary(Engagement.Shape)),
                Narrative = engagementSummary.Description
            };

            _inventory.AddResult(result);

            var forgedCrystals = _forgeManager.ForgeCluster(
                new List<(int threshold, bool isPositive, IReadOnlyDictionary<ToneType,int> facets, string narrative)>
                {
                    (
                        result.Threshold,
                        result.Bias.Bias == substrate_shared.Registries.enums.Bias.Positive,
                        result.Tones,
                        result.Narrative
                    )
                },
                new List<TraitCrystal>()
            );

            foreach (var crystal in forgedCrystals)
                _inventory.AddCrystal(crystal);

            var inventorySummary = _forgeManager.SummarizeInventory(forgedCrystals);

            // 🔹 Build composite summary with both engagement + inventory
            var composite = new CompositeSummary("Engagement Report", "Engagement + Inventory outcomes");
            composite.AddSummary(engagementSummary);
            composite.AddSummary(inventorySummary);

            return composite;
        }

        public ISummary Finalize() => Engagement.Finalize();
    }
}
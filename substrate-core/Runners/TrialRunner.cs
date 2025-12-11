using System;
using System.Collections.Generic;
using substrate_core.Managers;
using substrate_core.Models.Results;
using substrate_core.Models.Summaries;
using substrate_core.Models.Summaries.Types;
using substrate_shared.interfaces;
using substrate_shared.Mappers;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Traits.Base;

namespace substrate_core.Runners
{
    /// <summary>
    /// Runner responsible for executing trial engagements.
    /// References IEngagement and produces ISummary outputs.
    /// </summary>
    public class TrialRunner : IRunner
    {
        public IEngagement Engagement { get; }
        private readonly InventoryManager _inventory;
        private readonly CrystalForgeManager _forgeManager;

        public TrialRunner(InventoryManager inventory, IEngagement trialEngagement)
        {
            Engagement = trialEngagement; // 🔹 Always via IEngagement
            _inventory = inventory;
            _forgeManager = new CrystalForgeManager(inventory);
        }

        public ISummary Run(int ticks = 1)
        {
            Engagement.ResolveStep(ticks);

            // 🔹 Finalize trial into ISummary
            var trialSummary = Engagement.Finalize();

            // 🔹 Create EngagementResult for traceability
            var result = new EngagementResult
            {
                EngagementId = Guid.NewGuid(),
                Threshold = Engagement.Shape.Values.Count, // Example: threshold from shape size
                Bias = new BiasDescriptor
                {
                    Bias = trialSummary.Outcome switch
                    {
                        DuelOutcome.Recovery   => substrate_shared.Registries.enums.Bias.Positive,
                        DuelOutcome.Collapse   => substrate_shared.Registries.enums.Bias.Negative,
                        DuelOutcome.Stalemate  => substrate_shared.Registries.enums.Bias.Neutral,
                        _                      => substrate_shared.Registries.enums.Bias.Mixed
                    },
                    Narrative = trialSummary.Description
                },
                Shape = Engagement.Shape,
                Narrative = trialSummary.Description
            };

            // 🔹 Store result in inventory
            _inventory.AddResult(result);

            // 🔹 Forge crystals from result
            var forgedCrystals = _forgeManager.ForgeCluster(
                new List<(int threshold, bool isPositive, IReadOnlyDictionary<ToneType,int> facets, string narrative)>
                {
                    (
                        result.Threshold,
                        result.Bias.Bias == substrate_shared.Registries.enums.Bias.Positive,
                        FacetToneMapper.ToToneDictionary(result.Shape),
                        result.Narrative
                    )
                },
                new List<TraitCrystal>()
            );

            foreach (var crystal in forgedCrystals)
                _inventory.AddCrystal(crystal);

            // 🔹 Generate inventory summary
            var inventorySummary = _forgeManager.SummarizeInventory(forgedCrystals);

            // 🔹 Build composite summary with both trial + inventory
            var composite = new CompositeSummary("Trial Engagement Report", "Trial + Inventory outcomes");
            composite.AddSummary(trialSummary);
            composite.AddSummary(inventorySummary);

            return composite;
        }

        public ISummary Finalize() => Engagement.Finalize();
    }
}
using System;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.Summaries;
using substrate_shared.Traits.Base;
using substrate_core.Engagements.Results;
using substrate_core.Managers;
using substrate_core.Summaries.Types;
using substrate_shared;
using substrate_shared.Mappers;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_core.runners
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
            ISummary trialSummary = Engagement.Finalize();

            // 🔹 Create EngagementResult for traceability
            var result = new EngagementResult
            {
                EngagementId = Guid.NewGuid(),
                Threshold = Engagement.Shape.Values.Count, // Example: threshold from shape size
                Bias = new BiasDescriptor
                {
                    Bias = trialSummary.Outcome switch
                    {
                        DuelOutcome.Recovery => Bias.Positive,
                        DuelOutcome.Collapse => Bias.Negative,
                        DuelOutcome.Stalemate => Bias.Neutral,
                        _ => Bias.Mixed
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
                        result.Bias.Bias == Bias.Positive,
                        FacetToneMapper.ToToneDictionary(result.Shape), // 🔹 conversion here
                        result.Narrative
                    )
                },
                new List<TraitCrystal>()
            );


            foreach (var crystal in forgedCrystals)
                _inventory.AddCrystal(crystal);

            // 🔹 Generate inventory summary
            var inventorySummary = _forgeManager.SummarizeInventory(forgedCrystals);

            // 🔹 Return composite summary (trial + inventory)
            return new CompositeSummary(trialSummary, inventorySummary);
        }

        public ISummary Finalize() => Engagement.Finalize();
    }
}
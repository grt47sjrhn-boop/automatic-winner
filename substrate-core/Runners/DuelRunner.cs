using System;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.Summaries;
using substrate_shared.Traits.Base;
using substrate_shared.Registries.enums;
using substrate_core.Engagements.Results;
using substrate_core.Managers;
using substrate_core.Summaries.Types;
using substrate_shared.Mappers;
using substrate_shared.structs;

namespace substrate_core.runners
{
    /// <summary>
    /// Runner responsible for executing duel engagements.
    /// References IEngagement and produces ISummary outputs.
    /// </summary>
    public class DuelRunner : IRunner
    {
        public IEngagement Engagement { get; }
        private readonly InventoryManager _inventory;
        private readonly CrystalForgeManager _forgeManager;

        public DuelRunner(InventoryManager inventory, IEngagement duelEngagement)
        {
            Engagement = duelEngagement;
            _inventory = inventory;
            _forgeManager = new CrystalForgeManager(inventory);
        }

        public ISummary Run(int ticks = 1)
        {
            Engagement.ResolveStep(ticks);

            // 🔹 Finalize duel into ISummary
            ISummary duelSummary = Engagement.Finalize();

            // 🔹 Create EngagementResult for traceability
            var result = new EngagementResult
            {
                EngagementId = Guid.NewGuid(),
                Threshold = Engagement.Shape.Values.Count, // Example threshold logic
                Bias = new BiasDescriptor
                {
                    Bias = duelSummary.Outcome switch
                    {
                        DuelOutcome.Recovery   => Bias.Positive,
                        DuelOutcome.Collapse   => Bias.Negative,
                        DuelOutcome.Stalemate  => Bias.Neutral,
                        DuelOutcome.Unresolved => Bias.Mixed,
                        _                      => Bias.Mixed
                    },
                    Narrative = duelSummary.Description
                },
                Shape = Engagement.Shape,
                Tones = new Dictionary<ToneType,int>(FacetToneMapper.ToToneDictionary(Engagement.Shape)),
                Narrative = duelSummary.Description
            };

            // 🔹 Store result in inventory
            _inventory.AddResult(result);

            // 🔹 Forge crystals from tone distribution
            var forgedCrystals = _forgeManager.ForgeCluster(
                new List<(int threshold, bool isPositive, IReadOnlyDictionary<ToneType,int> facets, string narrative)>
                {
                    (
                        result.Threshold,
                        result.Bias.Bias == Bias.Positive,
                        result.Tones,
                        result.Narrative
                    )
                },
                new List<TraitCrystal>()
            );

            foreach (var crystal in forgedCrystals)
                _inventory.AddCrystal(crystal);

            // 🔹 Generate inventory summary
            var inventorySummary = _forgeManager.SummarizeInventory(forgedCrystals);

            // 🔹 Return composite summary (duel + inventory)
            return new CompositeSummary(duelSummary, inventorySummary);
        }

        public ISummary Finalize() => Engagement.Finalize();
    }
}
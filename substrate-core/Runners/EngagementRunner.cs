using System;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.Summaries;
using substrate_shared.Traits.Base;
using substrate_shared.Registries.enums;
using substrate_core.Engagements.Results;
using substrate_core.Managers;
using substrate_core.Summaries.Types;
using substrate_shared.Managers;
using substrate_shared.Mappers;
using substrate_shared.structs;

namespace substrate_core.runners
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
                        DuelOutcome.Recovery   => Bias.Positive,
                        DuelOutcome.Collapse   => Bias.Negative,
                        DuelOutcome.Stalemate  => Bias.Neutral,
                        DuelOutcome.Unresolved => Bias.Mixed,
                        _                      => Bias.Mixed
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
                        result.Bias.Bias == Bias.Positive,
                        result.Tones,
                        result.Narrative
                    )
                },
                new List<TraitCrystal>()
            );

            foreach (var crystal in forgedCrystals)
                _inventory.AddCrystal(crystal);

            var inventorySummary = _forgeManager.SummarizeInventory(forgedCrystals);

            return new CompositeSummary(engagementSummary, inventorySummary);
        }

        public ISummary Finalize() => Engagement.Finalize();
    }
}
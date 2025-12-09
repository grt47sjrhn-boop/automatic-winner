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
    /// Runner responsible for executing dialogue engagements.
    /// Produces EngagementResult and CompositeSummary.
    /// </summary>
    public class DialogueRunner : IRunner
    {
        public IEngagement Engagement { get; }
        private readonly InventoryManager _inventory;
        private readonly CrystalForgeManager _forgeManager;

        public DialogueRunner(InventoryManager inventory, IEngagement dialogueEngagement)
        {
            Engagement = dialogueEngagement;
            _inventory = inventory;
            _forgeManager = new CrystalForgeManager(inventory);
        }

        public ISummary Run(int ticks = 1)
        {
            Engagement.ResolveStep(ticks);

            var dialogueSummary = Engagement.Finalize();

            var result = new EngagementResult
            {
                EngagementId = Guid.NewGuid(),
                Threshold = Engagement.Shape.Values.Count,
                Bias = new BiasDescriptor
                {
                    Bias = dialogueSummary.Outcome switch
                    {
                        DuelOutcome.Recovery   => Bias.Positive,
                        DuelOutcome.Collapse   => Bias.Negative,
                        DuelOutcome.Stalemate  => Bias.Neutral,
                        DuelOutcome.Unresolved => Bias.Mixed,
                        _                      => Bias.Mixed
                    },
                    Narrative = dialogueSummary.Description
                },
                Shape = Engagement.Shape,
                Tones = new Dictionary<ToneType,int>(FacetToneMapper.ToToneDictionary(Engagement.Shape)),
                Narrative = dialogueSummary.Description
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

            return new CompositeSummary(dialogueSummary, inventorySummary);
        }

        public ISummary Finalize() => Engagement.Finalize();
    }
}
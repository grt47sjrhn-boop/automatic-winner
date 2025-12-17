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
                        DuelOutcome.Recovery   => substrate_shared.Registries.enums.Bias.Positive,
                        DuelOutcome.Collapse   => substrate_shared.Registries.enums.Bias.Negative,
                        DuelOutcome.Stalemate  => substrate_shared.Registries.enums.Bias.Neutral,
                        DuelOutcome.Unresolved => substrate_shared.Registries.enums.Bias.Mixed,
                        _                      => substrate_shared.Registries.enums.Bias.Mixed
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
                        result.Bias.Bias == substrate_shared.Registries.enums.Bias.Positive,
                        result.Tones,
                        result.Narrative
                    )
                },
                []
            );

            foreach (var crystal in forgedCrystals)
                _inventory.AddCrystal(crystal);

            var inventorySummary = _forgeManager.SummarizeInventory(forgedCrystals);

            // 🔹 Build composite summary with both dialogue + inventory
            var composite = new CompositeSummary("Dialogue Engagement Report", "Dialogue + Inventory outcomes");
            composite.AddSummary(dialogueSummary);
            composite.AddSummary(inventorySummary);

            return composite;
        }

        public ISummary Finalize() => Engagement.Finalize();
    }
}
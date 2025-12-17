using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Managers;
using substrate_core.Models.Engagements.Enums;
using substrate_core.Models.Engagements.Types;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Managers;
using substrate_shared.structs;

namespace substrate_core.Runners.Factories
{
    public static class RunnerFactory
    {
        public static IRunner Create(
            EngagementType type,
            InventoryManager inventory,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null,
            IEnumerable<BiasVector>? participants = null,
            IEnumerable<DuelEngagement>? duels = null)
        {
            IEngagement engagement = type switch
            {
                // Duel: use provided participants or generate two randoms
                EngagementType.Duel => new DuelEngagement(
                    toneManager,
                    rarityManager,
                    participants?.ElementAtOrDefault(0) ?? BiasVector.GenerateRandom(),
                    participants?.ElementAtOrDefault(1) ?? BiasVector.GenerateRandom(),
                    biasSeedId
                ),

                // Dialogue: seed‑driven, no participants needed
                EngagementType.Dialogue => new DialogueEngagement(
                    inventory,
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                // Trial: use provided duels or generate three random ones
                EngagementType.Trial => new TrialEngagement(
                    inventory,
                    duels ?? Enumerable.Range(0, 3).Select(_ =>
                        new DuelEngagement(
                            toneManager,
                            rarityManager,
                            BiasVector.GenerateRandom(),
                            BiasVector.GenerateRandom(),
                            biasSeedId
                        )
                    ),
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                // Ritual: use provided participants or generate three randoms
                EngagementType.Ritual => new RitualEngagement(
                    inventory,
                    participants ??
                    [
                        BiasVector.GenerateRandom(),
                        BiasVector.GenerateRandom(),
                        BiasVector.GenerateRandom()
                    ],
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported engagement type")
            };

            // Always wrap in EngagementRunner
            return new EngagementRunner(inventory, engagement);
        }
    }
}
using System;
using System.Collections.Generic;
using substrate_core.Engagements.Types;
using substrate_core.Managers;
using substrate_core.runners;
using substrate_shared.Engagements.Enums;
using substrate_shared.interfaces;
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
                // Build a DuelEngagement from two BiasVectors (randomized if none provided)
                EngagementType.Duel => new DuelEngagement(
                    inventory,
                    BiasVector.GenerateRandom(),
                    BiasVector.GenerateRandom(),
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                // Dialogue typically resolves around a seed; adapt as needed
                EngagementType.Dialogue => new DialogueEngagement(
                    inventory,
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                // Trial aggregates a series of duels; construct if none provided
                EngagementType.Trial => new TrialEngagement(
                    inventory,
                    duels ?? new[]
                    {
                        new DuelEngagement(
                            inventory,
                            BiasVector.GenerateRandom(),
                            BiasVector.GenerateRandom(),
                            biasManager,
                            facetManager,
                            toneManager,
                            rarityManager
                        ),
                        new DuelEngagement(
                            inventory,
                            BiasVector.GenerateRandom(),
                            BiasVector.GenerateRandom(),
                            biasManager,
                            facetManager,
                            toneManager,
                            rarityManager
                        ),
                        new DuelEngagement(
                            inventory,
                            BiasVector.GenerateRandom(),
                            BiasVector.GenerateRandom(),
                            biasManager,
                            facetManager,
                            toneManager,
                            rarityManager
                        )
                    },
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                // Ritual aggregates participants (BiasVectors); construct if none provided
                EngagementType.Ritual => new RitualEngagement(
                    inventory,
                    participants ?? new[]
                    {
                        BiasVector.GenerateRandom(),
                        BiasVector.GenerateRandom(),
                        BiasVector.GenerateRandom()
                    },
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported engagement type")
            };

            // Always run via the generic EngagementRunner to unify the pipeline
            return new EngagementRunner(inventory, engagement);
        }
    }
}
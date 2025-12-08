using System;
using System.Collections.Generic;
using substrate_core.Engagements.Types;
using substrate_core.Managers;
using substrate_shared.Engagements.Enums;
using substrate_shared.interfaces;
using substrate_shared.structs;

namespace substrate_core.Engagements.Factories
{
    public static class EngagementFactory
    {
        public static IEngagement Create(
            EngagementType type,
            InventoryManager inventory,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null,
            BiasVector? duelistA = null,
            BiasVector? duelistB = null,
            IEnumerable<DuelEngagement>? duels = null,
            IEnumerable<BiasVector>? participants = null)
        {
            return type switch
            {
                EngagementType.Duel => new DuelEngagement(
                    inventory,
                    duelistA ?? BiasVector.GenerateRandom(),
                    duelistB ?? BiasVector.GenerateRandom(),
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                EngagementType.Dialogue => new DialogueEngagement(
                    inventory,
                    biasManager,
                    facetManager,
                    toneManager,
                    rarityManager,
                    biasSeedId
                ),

                EngagementType.Trial => new TrialEngagement(
                    inventory,
                    duels ?? new List<DuelEngagement>
                    {
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

                EngagementType.Ritual => new RitualEngagement(
                    inventory,
                    participants ?? new List<BiasVector>
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

                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public static ISummary RunAndFinalize(
            EngagementType type,
            InventoryManager inventory,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null,
            int ticks = 1,
            BiasVector? duelistA = null,
            BiasVector? duelistB = null,
            IEnumerable<DuelEngagement>? duels = null,
            IEnumerable<BiasVector>? participants = null)
        {
            var engagement = Create(
                type,
                inventory,
                biasManager,
                facetManager,
                toneManager,
                rarityManager,
                biasSeedId,
                duelistA,
                duelistB,
                duels,
                participants
            );

            // Advance engagement by ticks
            engagement.ResolveStep(ticks);

            // Finalize and return polymorphic summary
            return engagement.Finalize();
        }
    }
}
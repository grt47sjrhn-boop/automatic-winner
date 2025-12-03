using System.Collections.Generic;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves trigger events based on crystallization, fragmentation, trait activations, and constellation formation.
    /// Produces TriggerSummary for contributor-facing narratability.
    /// </summary>
    public class TriggerResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            var tickId = vb.TickId;

            // Get summaries
            var delta       = vb.GetSummary<DeltaSummary>();
            var traits      = vb.GetSummary<TraitSummary>();
            var persistence = vb.GetSummary<PersistenceSummary>();
            var volatility  = vb.GetSummary<VolatilitySummary>();

            float persistenceVal = persistence?.Current ?? 0f;
            float volatilityVal  = volatility?.Volatility ?? 0f;

            var events = new List<TriggerEvent>();

            var hyp  = DebugOverlay.SafeFloat(delta.Hypotenuse);
            var area = DebugOverlay.SafeFloat(delta.Area);
            var crystallizationScore = hyp * area;

            // Crystallization attempt
            if (crystallizationScore > 50f)
            {
                events.Add(new TriggerEvent
                {
                    Type        = TriggerType.CrystallizationAttempt,
                    Description = $"Crystal formed (score={crystallizationScore:F2}), bias tilted toward {vb.Legacy}",
                    Magnitude   = hyp,
                    Score       = crystallizationScore,
                    TickId      = tickId,
                    Persistence = persistenceVal,
                    Volatility  = volatilityVal
                });
            }
            // Fragmentation attempt
            else if (crystallizationScore < -50f)
            {
                events.Add(new TriggerEvent
                {
                    Type        = TriggerType.FragmentationAttempt,
                    Description = $"Fragmentation surge (score={crystallizationScore:F2}), legacy stressed toward {vb.Legacy}",
                    Magnitude   = hyp,
                    Score       = crystallizationScore,
                    TickId      = tickId,
                    Persistence = persistenceVal,
                    Volatility  = volatilityVal
                });
            }

            // Trait activations (from TraitSummary.ActiveTraitIds/Labels)
            if (traits != null)
            {
                foreach (var id in traits.ActiveTraitIds)
                {
                    events.Add(new TriggerEvent
                    {
                        Type        = TriggerType.TraitActivation,
                        Description = $"{id} awakened",
                        Magnitude   = 1f, // you can extend TraitSummary to carry weights if needed
                        Score       = 1f,
                        TickId      = tickId,
                        Persistence = persistenceVal,
                        Volatility  = volatilityVal
                    });
                }
            }

            // Constellation formation (from TraitSummary.CrystallizedCount)
            if (traits != null && traits.CrystallizedCount >= 3)
            {
                events.Add(new TriggerEvent
                {
                    Type        = TriggerType.ConstellationFormation,
                    Description = $"Constellation formed with {traits.CrystallizedCount} crystallized traits, weaving mythic resonance",
                    Magnitude   = traits.CrystallizedCount,
                    Score       = traits.CrystallizedCount,
                    TickId      = tickId,
                    Persistence = persistenceVal,
                    Volatility  = volatilityVal
                });
            }

            

            // Build TriggerSummary
            var summary = new TriggerSummary
            {
                TickId = tickId,
                Events = events,
                Count  = events.Count
            };
            
            DebugOverlay.LogTrigger(vb, crystallizationScore, delta, summary);

            vb.AddSummary(summary);
            return new ResolutionResult(vb);
        }
    }
}
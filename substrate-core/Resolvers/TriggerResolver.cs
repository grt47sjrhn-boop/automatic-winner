using System.Linq;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    public class TriggerResolver : IResolver
    {
        public VectorBias Resolve(VectorBias vb, Mood mv)
        {
            vb.TriggerEvents ??= new System.Collections.Generic.List<TriggerEvent>();
            vb.Traits ??= new System.Collections.Generic.List<Trait>();

            int tickId = vb.TickId;

            float hyp = DebugOverlay.SafeFloat(vb.Hypotenuse);
            float area = DebugOverlay.SafeFloat(vb.Area);
            float crystallizationScore = hyp * area;

            // Crystallization attempt
            if (crystallizationScore > 50f)
            {
                vb.TriggerEvents.Add(new TriggerEvent
                {
                    Type = TriggerType.CrystallizationAttempt,
                    Description = $"Crystal formed (score={crystallizationScore:F2}), bias tilted toward {vb.Legacy}",
                    Magnitude = hyp,
                    Score = crystallizationScore,
                    TickId = tickId
                });

                foreach (var trait in vb.Traits.Where(t => !t.IsCrystallized && DebugOverlay.SafeFloat(t.Weight) > 0.5f))
                    trait.IsCrystallized = true;
            }

            // Trait activations
            foreach (var trait in vb.Traits.Where(t => t.State == TraitState.Active))
            {
                float w = DebugOverlay.SafeFloat(trait.Weight);
                vb.TriggerEvents.Add(new TriggerEvent
                {
                    Type = TriggerType.TraitActivation,
                    Description = $"{trait.Id} awakened",
                    Magnitude = w,
                    Score = w,
                    TickId = tickId
                });
            }

            // Constellation formation
            int crystallizedCount = vb.Traits.Count(t => t.IsCrystallized);
            if (crystallizedCount >= 3)
            {
                float constellationMagnitude = DebugOverlay.SafeFloat(vb.Traits.Sum(t => DebugOverlay.SafeFloat(t.Weight)));
                vb.TriggerEvents.Add(new TriggerEvent
                {
                    Type = TriggerType.ConstellationFormation,
                    Description = $"Constellation formed with {crystallizedCount} crystallized traits, weaving mythic resonance",
                    Magnitude = constellationMagnitude,
                    Score = constellationMagnitude,
                    TickId = tickId
                });
            }

            // Unified debug logging
            DebugOverlay.LogTrigger(vb, crystallizationScore);

            return vb;
        }
    }
}
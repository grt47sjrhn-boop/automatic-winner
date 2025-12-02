using System.Linq;
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
    /// Produces updated VectorBias plus a DeltaSummary snapshot for persistence/narration.
    /// </summary>
    public class TriggerResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            vb.TriggerEvents ??= new System.Collections.Generic.List<TriggerEvent>();
            vb.Traits ??= new System.Collections.Generic.List<Trait>();

            var tickId = vb.TickId;

            var hyp = DebugOverlay.SafeFloat(vb.Hypotenuse);
            var area = DebugOverlay.SafeFloat(vb.Area);
            var crystallizationScore = hyp * area;

            // Crystallization attempt
            if (crystallizationScore > 50f)
            {
                vb.TriggerEvents.Add(new TriggerEvent
                {
                    Type        = TriggerType.CrystallizationAttempt,
                    Description = $"Crystal formed (score={crystallizationScore:F2}), bias tilted toward {vb.Legacy}",
                    Magnitude   = hyp,
                    Score       = crystallizationScore,
                    TickId      = tickId,
                    Persistence = vb.Persistence,
                    Volatility  = vb.Volatility
                });

                foreach (var trait in vb.Traits.Where(t => !t.IsCrystallized && DebugOverlay.SafeFloat(t.Weight) > 0.5f))
                    trait.IsCrystallized = true;
            }
            // Fragmentation attempt
            else if (crystallizationScore < -50f)
            {
                vb.TriggerEvents.Add(new TriggerEvent
                {
                    Type        = TriggerType.FragmentationAttempt,
                    Description = $"Fragmentation surge (score={crystallizationScore:F2}), legacy stressed toward {vb.Legacy}",
                    Magnitude   = hyp,
                    Score       = crystallizationScore,
                    TickId      = tickId,
                    Persistence = vb.Persistence,
                    Volatility  = vb.Volatility
                });

                foreach (var trait in vb.Traits.Where(t => t.IsCrystallized && DebugOverlay.SafeFloat(t.Weight) < 0.4f))
                    trait.IsCrystallized = false;
            }

            // Trait activations
            foreach (var trait in vb.Traits.Where(t => t.State == TraitState.Active))
            {
                var w = DebugOverlay.SafeFloat(trait.Weight);
                vb.TriggerEvents.Add(new TriggerEvent
                {
                    Type        = TriggerType.TraitActivation,
                    Description = $"{trait.Id} awakened",
                    Magnitude   = w,
                    Score       = w,
                    TickId      = tickId,
                    Persistence = vb.Persistence,
                    Volatility  = vb.Volatility
                });
            }

            // Constellation formation
            int crystallizedCount = vb.Traits.Count(t => t.IsCrystallized);
            if (crystallizedCount >= 3)
            {
                var constellationMagnitude = DebugOverlay.SafeFloat(vb.Traits.Sum(t => DebugOverlay.SafeFloat(t.Weight)));
                vb.TriggerEvents.Add(new TriggerEvent
                {
                    Type        = TriggerType.ConstellationFormation,
                    Description = $"Constellation formed with {crystallizedCount} crystallized traits, weaving mythic resonance",
                    Magnitude   = constellationMagnitude,
                    Score       = constellationMagnitude,
                    TickId      = tickId,
                    Persistence = vb.Persistence,
                    Volatility  = vb.Volatility
                });
            }

            // Unified debug logging
            DebugOverlay.LogTrigger(vb, crystallizationScore);

            // Build DeltaSummary snapshot for consistency
            var summary = new DeltaSummary
            {
                DeltaAxis   = mv.MoodAxis - vb.MoodAxis,
                Magnitude   = mv.MagnitudeFrom(vb.CurrentMood),
                Hypotenuse  = vb.Hypotenuse,
                Area        = vb.Area,
                AngleTheta  = vb.AngleTheta,
                SinTheta    = vb.SinTheta,
                CosTheta    = vb.CosTheta,
                TanTheta    = vb.TanTheta
            };

            return new ResolutionResult(vb, summary);
        }
    }
}
using System;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// PersonalityResolver interprets persistence trajectories from VectorBias
    /// into narratable personality states. It applies hardened bias logic
    /// and resilience/scar modifiers to incoming moods.
    /// Dependencies on persistence to calculate personality
    /// </summary>
    public class PersonalityResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            var persistence = vb.Persistence;

            var resolvedState = Neutral;
            var hardenedBias = HardenedBiasType.None;

            // 1) Resolve personality state directly from persistence flags
            if (persistence.IsRecovering)
                resolvedState = Recovering;
            else if (persistence.IsFracturing)
                resolvedState = Fracturing;
            else
                resolvedState = persistence.Trajectory switch
                {
                    BiasTrajectory.SettlingResonance => Resonating,
                    BiasTrajectory.SettlingWound => SettlingScar,
                    _ => resolvedState
                };

            switch (resolvedState)
            {
                // 2) Hardened overlay logic
                case Recovering when
                    persistence.Current > 10f && persistence.IsIncreasing:
                    resolvedState = Hardened;
                    hardenedBias = HardenedBiasType.Resilient; // resists negatives
                    vb.Persistence.GrantResilienceBonus(vb.CurrentMood.MoodType.GetNarrativeGroup());
                    break;
                case Fracturing when
                    persistence.Current < -10f && persistence.IsDecreasing:
                    resolvedState = Hardened;
                    hardenedBias = HardenedBiasType.Scarred; // resists positives
                    break;
                case Neutral:
                case SettlingScar:
                case Resonating:
                case Hardened:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // 3) Apply hardened modifiers to incoming mood
            vb.Persistence.ApplyModifiers(mv, hardenedBias);

            // 4) Return resolution result with narratable state
            return new ResolutionResult(vb, new DeltaSummary
            {
                DeltaAxis = persistence.Delta,
                Magnitude = persistence.Current,
                Hypotenuse = vb.Hypotenuse,
                Area = vb.Area,
                AngleTheta = vb.AngleTheta,
                SinTheta = vb.SinTheta,
                CosTheta = vb.CosTheta,
                TanTheta = vb.TanTheta
            })
            {
                PersonalityState = resolvedState,
                HardenedBias = hardenedBias
            };
        }
    }
}
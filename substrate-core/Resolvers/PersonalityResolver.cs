using System;
using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    public class PersonalityResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            var persistence = vb.GetSummary<PersistenceSummary>();
            var delta       = vb.GetSummary<DeltaSummary>();

            if (persistence == null)
                throw new InvalidOperationException("PersonalityResolver requires PersistenceSummary.");
            if(delta == null)
                throw new InvalidOperationException("PersonalityResolver requires DeltaSummary.");
            var resolvedState = PersonalityState.Neutral;
            var hardenedBias  = HardenedBiasType.None;
            var traceLogs     = new List<string>();

            // 1) Resolve personality state directly from persistence flags
            if (persistence.IsRecovering)
                resolvedState = PersonalityState.Recovering;
            else if (persistence.IsFracturing)
                resolvedState = PersonalityState.Fracturing;
            else
            {
                resolvedState = persistence.Trajectory switch
                {
                    BiasTrajectory.SettlingResonance => PersonalityState.Resonating,
                    BiasTrajectory.SettlingWound     => PersonalityState.SettlingScar,
                    _                                => resolvedState
                };
            }

            // 2) Hardened overlay logic
            switch (resolvedState)
            {
                case PersonalityState.Recovering when persistence.Current > 10f && persistence.IsIncreasing:
                    resolvedState = PersonalityState.Hardened;
                    hardenedBias  = HardenedBiasType.Resilient;
                    traceLogs.Add("Recovered strongly: hardened as Resilient.");
                    break;

                case PersonalityState.Fracturing when persistence.Current < -10f && persistence.IsDecreasing:
                    resolvedState = PersonalityState.Hardened;
                    hardenedBias  = HardenedBiasType.Scarred;
                    traceLogs.Add("Fractured deeply: hardened as Scarred.");
                    break;

                case PersonalityState.Neutral:
                    traceLogs.Add("Neutral state: no modifiers applied, personality stable.");
                    break;

                case PersonalityState.SettlingScar:
                    traceLogs.Add("SettlingScar: persistence trending downward, scar influence present.");
                    break;

                case PersonalityState.Resonating:
                    traceLogs.Add("Resonating: persistence trending upward, resonance influence present.");
                    break;

                case PersonalityState.Hardened:
                    traceLogs.Add($"Hardened: bias locked as {hardenedBias}.");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // 3) Build PersonalitySummary
            var summary = new PersonalitySummary
            {
                TickId         = vb.TickId,
                State          = resolvedState,
                HardenedBias   = hardenedBias,
                PersistenceVal = persistence.Current,
                DeltaAxis      = delta?.DeltaAxis ?? mv.MoodAxis,
                Trajectory     = persistence.Trajectory.ToString(),
                AngleTheta     = delta?.AngleTheta ?? 0f,
                ResilienceBonusApplied = (resolvedState == PersonalityState.Hardened && hardenedBias == HardenedBiasType.Resilient),
                ModifiersApplied       = hardenedBias != HardenedBiasType.None,
                TraceLogs              = traceLogs
            };

            vb.AddSummary(summary);
            return new ResolutionResult(vb);
        }
    }
}
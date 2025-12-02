using System;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_shared.types.models
{
    /// <summary>
    /// PersistenceState tracks persistence as a narratable object:
    /// - Current and Previous values
    /// - Delta and directional flags
    /// - BiasTrajectory (scar settling, resonance recovery, neutral drift)
    /// - Recovery values when motivators align
    /// </summary>
    public class PersistenceState
    {
        public float Current { get; private set; }
        public float Previous { get; private set; }
        public float Delta => Current - Previous;

        // Directional flags
        public bool IsIncreasing => Delta > 0;
        public bool IsDecreasing => Delta < 0;

        // Recovery / fracture flags
        public bool IsRecovering { get; private set; }
        public bool IsFracturing { get; private set; }
        public bool IsDrifting => !IsRecovering && !IsFracturing;

        // Bias trajectory marker
        public BiasTrajectory Trajectory { get; private set; }

        // Recovery magnitude (if applicable)
        public float RecoveryValue { get; private set; }

        /// <summary>
        /// Update persistence state with new value, legacy lock, and resonance/scar ratio.
        /// Determines trajectory and recovery potential.
        /// </summary>
        public void Update(float newValue, TraitAffinity legacy, float resonanceScarRatio)
        {
            Previous = Current;
            Current = newValue;

            // Default trajectory
            Trajectory = BiasTrajectory.NeutralDrift;
            IsRecovering = false;
            IsFracturing = false;
            RecoveryValue = 0f;

            // Fracture settling: persistence trending downward and negative
            if (IsDecreasing && Current < 0)
            {
                Trajectory = BiasTrajectory.SettlingWound;
                IsFracturing = true;

                // Legacy lock can deepen fracture bias
                if (legacy == TraitAffinity.FracturedLegacy)
                    Current *= 1.1f; // amplify scar settling
            }

            // Resonance recovery: persistence trending upward and positive
            else if (IsIncreasing && Current > 0)
            {
                Trajectory = BiasTrajectory.SettlingResonance;
                IsRecovering = true;

                // Recovery value biased by resonance/scar ratio
                RecoveryValue = Delta * resonanceScarRatio;

                // Legacy lock can amplify recovery bias
                if (legacy == TraitAffinity.ResilientHarmony)
                    Current *= 1.1f;
            }
        }

        public void GrantResilienceBonus(string getNarrativeGroup)
        {
            throw new NotImplementedException();
        }

        public void ApplyModifiers(Mood mv, HardenedBiasType hardenedBias)
        {
            throw new NotImplementedException();
        }
    }
}
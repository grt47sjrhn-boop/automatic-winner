using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.StateMachines
{
    /// <summary>
    /// Personality is a higher-level construct fed by VectorBias.
    /// It interprets persistence trajectories, wounds/scars, recovery cycles,
    /// and hardened states into narratable personality arcs.
    /// </summary>
    public class PersonalityStateMachine
    {
        public PersonalityState CurrentState { get; private set; } = PersonalityState.Neutral;
        public HardenedBiasType HardenedBias { get; private set; } = HardenedBiasType.None;

        // Track wound source for resilience/scar bonuses
        public MoodType? WoundSource { get; private set; }
        public int ResilienceTicksRemaining { get; private set; }
        public float ResilienceBonus { get; private set; }

        /// <summary>
        /// Update personality state based on VectorBias persistence trajectory.
        /// </summary>
        public void Update(VectorBias vb)
        {
            var persistence = vb.Persistence;

            switch (CurrentState)
            {
                case PersonalityState.Neutral:
                    if (persistence.IsFracturing)
                    {
                        CurrentState = PersonalityState.Fracturing;
                        WoundSource = vb.CurrentMood.MoodType.GetNarrativeGroup();
                    }
                    else if (persistence.IsRecovering)
                    {
                        CurrentState = PersonalityState.Recovering;
                    }
                    break;

                case PersonalityState.Fracturing:
                    if (persistence.IsRecovering)
                    {
                        CurrentState = PersonalityState.Recovering;
                    }
                    else if (!persistence.IsRecovering && persistence.Current < -10)
                    {
                        CurrentState = PersonalityState.Hardened;
                        HardenedBias = HardenedBiasType.Wounded; // resists positives
                    }
                    break;

                case PersonalityState.Recovering:
                    if (persistence.IsFracturing)
                    {
                        CurrentState = PersonalityState.Fracturing;
                    }
                    else if (persistence.Current > 10 && persistence.IsIncreasing)
                    {
                        CurrentState = PersonalityState.Hardened;
                        HardenedBias = HardenedBiasType.Scarred; // resists negatives
                        HardenRecovery();
                    }
                    break;

                case PersonalityState.Hardened:
                    // Hardened is sticky; only extreme fracture can break it
                    if (persistence.IsFracturing && persistence.Current < -15)
                    {
                        CurrentState = PersonalityState.Fracturing;
                        HardenedBias = HardenedBiasType.None;
                    }
                    break;
            }
        }

        /// <summary>
        /// Grant resilience bonus for a few ticks after recovery hardens.
        /// </summary>
        private void HardenRecovery()
        {
            if (WoundSource != null)
            {
                ResilienceTicksRemaining = 3; // configurable
                ResilienceBonus = 1.25f;      // 25% boost against wound source
            }
        }

        /// <summary>
        /// Apply resilience or scar resistance to incoming moods.
        /// </summary>
        public void ApplyModifiers(Mood incoming)
        {
            if (CurrentState == PersonalityState.Hardened)
            {
                var group = incoming.MoodType.GetNarrativeGroup();

                if (HardenedBias == HardenedBiasType.Scarred &&
                    ResilienceTicksRemaining > 0 &&
                    WoundSource == group)
                {
                    // Resilient hardened resists negatives
                    incoming.Intensity *= ResilienceBonus;
                    ResilienceTicksRemaining--;
                }
                else if (HardenedBias == HardenedBiasType.Wounded && group.IsPositive())
                {
                    // Scarred hardened resists positives
                    incoming.Intensity *= 0.75f; // dampen positives
                }
            }
        }
    }

    public enum PersonalityState
    {
        Neutral,
        Fracturing,
        Recovering,
        SettlingScar,
        Resonating,
        Hardened
    }
}
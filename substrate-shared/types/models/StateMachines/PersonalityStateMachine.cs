using System;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.types.models.Profiles;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_shared.types.models.StateMachines
{
    /// <summary>
    /// Personality is a higher-level construct fed by summaries in PersonalityProfile.
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
        /// Update personality state based on profile summaries.
        /// </summary>
        public void Update(PersonalityProfile profile)
        {
            var persistence = profile.Summaries.OfType<PersistenceSummary>().FirstOrDefault();
            var toneCluster = profile.Summaries.OfType<ToneClusterSummary>().FirstOrDefault();
            var delta       = profile.Summaries.OfType<DeltaSummary>().FirstOrDefault();

            if (persistence == null)
                return;

            switch (CurrentState)
            {
                case PersonalityState.Neutral:
                    if (persistence.IsFracturing)
                    {
                        CurrentState = PersonalityState.Fracturing;

                        // Prefer Delta axis (float → MoodType), otherwise map toneCluster → MoodType
                        if (delta != null)
                        {
                            WoundSource = MoodTypeExtensions.Resolve(delta.DeltaAxis);
                        }
                        else if (toneCluster != null)
                        {
                            // Use first candidate tone, fallback to baseline
                            var candidateTone = toneCluster.BaseLineTones.FirstOrDefault().Tone ?? toneCluster.Baseline;
                            WoundSource = MapToneToMood(candidateTone);
                        }
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
                    else if (persistence is { IsRecovering: false, Current: < -10 })
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
                    else if (persistence is { Current: > 10, IsIncreasing: true })
                    {
                        CurrentState = PersonalityState.Hardened;
                        HardenedBias = HardenedBiasType.Scarred; // resists negatives
                        HardenRecovery();
                    }
                    break;

                case PersonalityState.Hardened:
                    if (persistence is { IsFracturing: true, Current: < -15 })
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
        public void ApplyModifiers(ref Mood incoming)
        {
            if (CurrentState == PersonalityState.Hardened)
            {
                var group = incoming.MoodType.GetNarrativeGroup();

                switch (HardenedBias)
                {
                    case HardenedBiasType.Scarred when
                        ResilienceTicksRemaining > 0 &&
                        WoundSource != null &&
                        WoundSource.Value.GetNarrativeGroup() == group:
                        // Scarred hardened resists negatives (temporary resilience bonus)
                        incoming.MoodAxis *= ResilienceBonus;
                        incoming.SetMood(MoodTypeExtensions.Resolve(incoming.MoodAxis));
                        ResilienceTicksRemaining--;
                        break;

                    case HardenedBiasType.Wounded when
                        incoming.MoodType.GetValence() == "Positive":
                        // Wounded hardened resists positives
                        incoming.MoodAxis *= 0.75f;
                        incoming.SetMood(MoodTypeExtensions.Resolve(incoming.MoodAxis));
                        break;

                    case HardenedBiasType.Resilient when
                        incoming.MoodType.GetValence() == "Negative":
                        // Resilient hardened resists negatives (mirror of Wounded)
                        incoming.MoodAxis *= 0.75f;
                        incoming.SetMood(MoodTypeExtensions.Resolve(incoming.MoodAxis));
                        break;

                    case HardenedBiasType.None:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private static MoodType MapToneToMood(NarrativeTone? nt)
        {
            if (nt == null) return MoodType.Neutral;

            return nt.Category.ToLowerInvariant() switch
            {
                "despair"   => MoodType.Sadness,
                "hostility" => MoodType.Irritation,
                "darkness"  => MoodType.Anxiety,
                "neutral"   => MoodType.Neutral,
                "anxiety"   => MoodType.Anxiety,
                "resonance" => MoodType.Happiness,
                "joy"       => MoodType.Happiness,
                "confidence"=> MoodType.Contentment,
                "love"      => MoodType.Contentment,
                "wonder"    => MoodType.Happiness,
                _           => MoodType.Neutral
            };
        }

    }
}
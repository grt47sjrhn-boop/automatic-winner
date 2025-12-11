using System;
using System.Linq;
using substrate_core.Models.Profiles;
using substrate_core.Models.Summaries;
using substrate_core.Models.Summaries.Types;
using substrate_shared.interfaces;
using substrate_shared.Profiles;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions;
using substrate_shared.Registries.Factories;
using substrate_shared.structs;
using substrate_shared.types;

// for GetScaleValue, ResolveByScale

namespace substrate_core.Models
{
    public class Duelist : IDuelist
    {
        public string Name { get; }
        public double Bias { get; private set; }
        public double Resilience { get; private set; }
        public int Wounds { get; private set; }
        public int Recoveries { get; private set; }
        public BiasVector BiasVector { get; set; }
        public double Difficulty { get; set; }
        // 🔹 Explicit seed tone override
        private ToneType? _seedTone;

        public Duelist(string name, double initialBias = 0.0, ToneType? seedTone = null, double difficulty = 1.0)
        {
            
            Name = name;
            Bias = initialBias;
            Difficulty = difficulty;
            Resilience = 1.0;
            Wounds = 0;
            Recoveries = 0;

            if (seedTone.HasValue)
            {
                SeedTone(seedTone.Value);
            }
            else
            {
                // ✅ Build BiasVector immediately from initialBias and Resilience
                var moodKey   = ResolveMoodFromBias(Bias);
                var magnitude = MapResilienceToMagnitude(Resilience + Math.Abs(Bias));
                var toneKey   = Bias > 0 ? ToneType.Joy
                    : Bias < 0 ? ToneType.Despairing
                    : ToneType.Neutral;

                BiasVector = BiasVectorFactory.FromToneAndMood(toneKey, moodKey, magnitude);
            }
        }

        // Apply duel outcome from EventSummary
        public void ApplyOutcome(ISummary summary, double difficulty)
        {
            if (summary is DuelEventSummary duelSummary && duelSummary.Type == SummaryType.Duel)
            {
                switch (duelSummary.Outcome)
                {
                    case DuelOutcome.Recovery:
                        Bias += (0.1 * Resilience) * difficulty;
                        Resilience += 0.05 * difficulty;
                        Recoveries++;
                        break;

                    case DuelOutcome.Collapse:
                        Bias -= (0.1 * (2.0 - Resilience)) * difficulty;
                        Resilience = Math.Max(0, Resilience - (0.05 * difficulty));
                        Wounds++;
                        break;

                    case DuelOutcome.Wound:
                        Bias -= (0.1 * (2.0 - Resilience)) * difficulty;
                        Resilience = Math.Max(0, Resilience - (0.05 * difficulty));
                        Wounds++;
                        break;

                    case DuelOutcome.Conflict:
                    case DuelOutcome.Equilibrium:
                        // Neutral adjustments
                        Bias += 0.0;
                        Resilience += 0.0;
                        break;
                }

                // ✅ Refresh BiasVector after every outcome
                var toneKey = _seedTone.HasValue
                    ? _seedTone.Value
                    : Bias > 0 ? ToneType.Joy
                        : Bias < 0 ? ToneType.Despairing
                            : ToneType.Neutral;

                var moodKey   = ResolveMoodFromBias(Bias);
                var magnitude = MapResilienceToMagnitude(Resilience + Math.Abs(Bias));

                BiasVector = BiasVectorFactory.FromToneAndMood(toneKey, moodKey, magnitude);
            }
        }

        public override string ToString()
        {
            return $"{Name} | Bias={Bias:F2}, Resilience={Resilience:F2}, Wounds={Wounds}, Recoveries={Recoveries}";
        }

        // 🔹 Convert Duelist state to a BiasVector
        public BiasVector ToBiasVector()
        {
            var toneKey = _seedTone.HasValue
                ? _seedTone.Value
                : Bias > 0 ? ToneType.Joy
                : Bias < 0 ? ToneType.Despairing
                : ToneType.Neutral;

            var moodKey = ResolveMoodFromBias(Bias);
            var magnitude = MapResilienceToMagnitude(Resilience);

            return BiasVectorFactory.FromToneAndMood(toneKey, moodKey, magnitude);
        }

        private static int MapResilienceToMagnitude(double resilience)
        {
            var clamped = Math.Max(0.0, Math.Min(resilience, 2.0));
            return (int)Math.Round(clamped * 10); // 0–20
        }

        private static MoodType ResolveMoodFromBias(double bias)
        {
            var clamped = Math.Max(-11, Math.Min(11, (int)Math.Round(bias * 10)));
            var entries = Enum.GetValues(typeof(MoodType)).Cast<MoodType>();
            return entries.FirstOrDefault(m => m.GetScaleValue() == clamped);
        }

        // 🔹 Seed duelist with a starting tone
        // 🔹 Seed duelist with a starting tone
        public void SeedTone(ToneType seedTone)
        {
            var biasAttr = seedTone.GetBias(); // returns your Bias enum

            switch (biasAttr)
            {
                case substrate_shared.Registries.enums.Bias.Positive:
                    Bias = 1.0;
                    Resilience = 1.5;
                    break;

                case substrate_shared.Registries.enums.Bias.Negative:
                    // Stronger collapse for abyssal tones
                    Bias = seedTone is ToneType.Forsaken or ToneType.Corrupted or ToneType.Doomed ? -2.0 : -1.0;
                    Resilience = 0.5;
                    break;

                case substrate_shared.Registries.enums.Bias.Mixed:
                    Bias = 0.0;
                    Resilience = 1.0;
                    break;

                case substrate_shared.Registries.enums.Bias.Neutral:
                default:
                    Bias = 0.0;
                    Resilience = 1.0;
                    break;
            }

            _seedTone = seedTone;

            // ✅ Build a BiasVector immediately so summaries have a valid tone
            var moodKey   = ResolveMoodFromBias(Bias);
            var magnitude = MapResilienceToMagnitude(Resilience);
            BiasVector    = BiasVectorFactory.FromToneAndMood(seedTone, moodKey, magnitude);
        }
    }
}
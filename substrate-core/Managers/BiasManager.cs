using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Enums;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Managers;
using substrate_shared.Profiles;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

// ✅ Correct import for OpponentProfile & OpponentProfiles

namespace substrate_core.Managers
{
    public class BiasManager : IBiasManager
    {
        private readonly Random _rng;

        public BiasManager(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        // 🔹 Summarize facets into a bias descriptor
        public BiasDescriptor Summarize(FacetDistribution shape)
        {
            var resilience = shape.Values[FacetType.Resilience];
            var conflict   = shape.Values[FacetType.Conflict];
            var harmony    = shape.Values[FacetType.Harmony];
            var radiance   = shape.Values[FacetType.Radiance];

            Bias bias;
            if (resilience > conflict) bias = Bias.Positive;
            else if (conflict > resilience) bias = Bias.Negative;
            else if (harmony > 0) bias = Bias.Neutral;
            else bias = Bias.Mixed;

            var notes = $"Resilience({resilience}), Conflict({conflict}), Harmony({harmony}), Radiance({radiance})";

            return new BiasDescriptor
            {
                Bias = bias,
                Narrative = $"Bias summary → {notes}"
            };
        }

        // 🔹 Combine facets with a seed bias
        public BiasDescriptor Combine(FacetDistribution shape, BiasDescriptor seedBias)
        {
            var combined = Summarize(shape);
            if (seedBias.Bias != Bias.Neutral)
                combined.Bias = seedBias.Bias;

            combined.Narrative += $" | Seed bias tilt → {seedBias.Bias}";
            return combined;
        }

        // 🔹 Resolve two vectors into one, optionally tilted by seed bias
        public BiasVector Resolve(BiasVector duelistA, BiasVector duelistB, BiasDescriptor bias)
        {
            var resolved = duelistA.SignedStrength >= duelistB.SignedStrength ? duelistA : duelistB;

            if (bias == null || bias.Bias == Bias.Neutral)
                return resolved;

            var tiltedTone = new NarrativeTone(
                resolved.Tone.Type,
                resolved.Tone.Label,
                resolved.Tone.Category,
                bias.Bias,
                resolved.Tone.Group
            );

            return new BiasVector(tiltedTone, resolved.Magnitude);
        }

        // 🔹 Simple opponent generator (legacy)
        public BiasVector GenerateOpponent()
        {
            var weightedTones = new List<ToneType>
            {
                ToneType.Hostile, ToneType.Hostile,
                ToneType.Critical, ToneType.Critical,
                ToneType.Despairing, ToneType.Despairing,
                ToneType.Wound, ToneType.Wound,
                ToneType.Corrupted, ToneType.Corrupted,
                ToneType.Doomed, ToneType.Doomed,

                ToneType.Resilient,
                ToneType.Radiant,
                ToneType.Joy,
                ToneType.Playful,

                ToneType.Harmonious,
                ToneType.Calm,
                ToneType.Reflective,
                ToneType.Equilibrium,
                ToneType.Conflict,
                ToneType.Composite
            };

            var randomTone = weightedTones[_rng.Next(weightedTones.Count)];
            var bias = (Bias)_rng.Next(Enum.GetValues(typeof(Bias)).Length);

            var tone = new NarrativeTone(
                randomTone,
                NarrativeGroup.Duel.ToString(),
                "Opponent",
                bias,
                NarrativeGroup.Duel.ToString() // ✅ fixed: pass enum, not string
            );

            var magnitude = (int)Math.Round(_rng.NextDouble() * 20 - 10);
            return new BiasVector(tone, magnitude);

        }

        public BiasVector GenerateOpponentWeighted(IOpponentProfile profile, EnvironmentMood mood, BiasDescriptor? tilt = null)
        {
            var pool = BuildWeightedTones(profile, mood);
            var toneType = pool[_rng.Next(pool.Count)];
            var bias = PickWeightedBias(profile.CollapseBiasFactor);
            var magnitude = PickMagnitude(profile);

            var tone = new NarrativeTone(
                toneType,
                profile.Label ?? NarrativeGroup.Duel.ToString(),
                profile.Category ?? "Opponent",
                bias,
                NarrativeGroup.Duel.ToString()
            );

            var vector = new BiasVector(tone, magnitude);

            if (tilt != null && tilt.Bias != Bias.Neutral)
            {
                var tiltedTone = new NarrativeTone(
                    tone.Type,
                    tone.Label,
                    tone.Category,
                    tilt.Bias,
                    tone.Group
                );
                vector = new BiasVector(tiltedTone, vector.Magnitude);
            }

            return vector;
        }
        

        // 🔹 Helpers for weighted generator
        private List<ToneType> BuildWeightedTones(IOpponentProfile p, EnvironmentMood? env)
        {
            var pool = new List<ToneType>();
            void add(ToneType t, int w) { for (var i = 0; i < w; i++) pool.Add(t); }

            foreach (var t in new[] { ToneType.Hostile, ToneType.Critical, ToneType.Despairing, ToneType.Wound, ToneType.Corrupted, ToneType.Doomed })
                add(t, (int)Math.Round(4 * p.CollapseBiasFactor));

            foreach (var t in new[] { ToneType.Resilient, ToneType.Radiant, ToneType.Joy, ToneType.Playful })
                add(t, (int)Math.Round(1 * p.RecoveryBiasFactor));

            foreach (var t in new[] { ToneType.Harmonious, ToneType.Calm, ToneType.Reflective, ToneType.Equilibrium, ToneType.Conflict, ToneType.Composite })
                add(t, (int)Math.Round(1 * p.NeutralBiasFactor));

            if (env.HasValue)
            {
                switch (env.Value)
                {
                    case EnvironmentMood.Storm:
                        pool.AddRange(Enumerable.Repeat(ToneType.Critical, 3));
                        pool.AddRange(Enumerable.Repeat(ToneType.Hostile, 3));
                        break;
                    case EnvironmentMood.Sanctuary:
                        pool.AddRange(Enumerable.Repeat(ToneType.Resilient, 3));
                        pool.AddRange(Enumerable.Repeat(ToneType.Harmonious, 2));
                        break;
                    case EnvironmentMood.Void:
                        pool.AddRange(Enumerable.Repeat(ToneType.Doomed, 4));
                        pool.AddRange(Enumerable.Repeat(ToneType.Corrupted, 3));
                        break;
                    case EnvironmentMood.Carnival:
                        pool.AddRange(Enumerable.Repeat(ToneType.Playful, 3));
                        pool.AddRange(Enumerable.Repeat(ToneType.Radiant, 2));
                        break;
                }
            }

            return pool.Count > 0 ? pool : new List<ToneType> { ToneType.Conflict };
        }

        private Bias PickWeightedBias(double collapseBiasFactor)
        {
            var weights = new List<Bias>();
            void add(Bias b, int w) { for (var i = 0; i < w; i++) weights.Add(b); }

            add(Bias.Negative, (int)Math.Round(3 * collapseBiasFactor));
            add(Bias.Mixed, 2);
            add(Bias.Neutral, 2);
            add(Bias.Positive, 1);

            return weights[_rng.Next(weights.Count)];
        }

        private int PickMagnitude(IOpponentProfile p)
        {
            double u() => _rng.NextDouble() - 0.5;
            var gaussianish = (u() + u() + u()) * 8.0;
            var scaled = gaussianish * p.MagnitudeVariance * p.DifficultyMultiplier;

            var mag = (int)Math.Round(Math.Max(-10, Math.Min(10, scaled)));
            if (mag == 0 && p.AggressionNudge) mag = _rng.Next(0, 2) == 0 ? -1 : 1;
            return mag;
        }
    }
}
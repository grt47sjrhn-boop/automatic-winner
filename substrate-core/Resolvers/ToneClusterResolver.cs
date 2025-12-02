using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.interfaces;
using substrate_shared.Registries;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Hybrid ToneClusterResolver:
    /// - Uses ToneRegistry weighted category selection for variation
    /// - Preserves pipeline: baseline → adjacents → complement → bias → persistence
    /// - Produces narratable trace logs for contributors
    /// </summary>
    public class ToneClusterResolver : IResolver
    {
        public static ToneResolutionResult ResolveTone(
            (Mood primary, List<(Mood mood, Tone tone)> adjacents, Mood tertiary) moodTuple,
            Mood complement,
            VectorBias bias,
            BiasMode biasMode = BiasMode.Deterministic,
            Random? rng = null)
        {
            rng ??= new Random();
            var result = new ToneResolutionResult();

            // 1. Baseline from weighted category selection
            string category = ResolveCategory(moodTuple.primary);
            var primaryTone = ToneRegistry.SelectWeighted(category);
            var secondaryTone = ToneRegistry.SelectWeighted(category);

            result.Baseline = Blend(primaryTone, secondaryTone, 0.5f);
            result.AddTrace($"[1] Baseline from {moodTuple.primary} ({category}): {result.Baseline}");

            // 2. Blend adjacents
            result.Blended = result.Baseline;
            if (moodTuple.adjacents != null)
            {
                foreach (var adj in moodTuple.adjacents)
                {
                    result.Blended = Blend(result.Blended, adj.tone);
                    result.AddTrace($"[2] Blended with {adj.mood}: {result.Blended}");
                }
            }

            // 3. Complement adjustment
            string compCategory = ResolveCategory(complement);
            var compTone = ToneRegistry.SelectWeighted(compCategory);
            result.ComplementAdjusted = Blend(result.Blended, compTone, 0.25f);
            result.AddTrace($"[3] Complement {complement} ({compCategory}): {result.ComplementAdjusted}");

            // 4. Bias gravity
            var preferredTone = BiasPreferredTone(bias);
            float strength = Math.Clamp(bias.Persistence / 10f, 0f, 1f);

            result.BiasAdjusted = ApplyBias(
                result.ComplementAdjusted,
                preferredTone,
                strength,
                biasMode,
                rng
            );

            result.AddTrace($"[4] Bias applied ({biasMode}, strength {strength}): {result.BiasAdjusted}");

            // 5. Persistence modifiers (legacy overrides removed)
            result.Final = result.BiasAdjusted;

            // Recover weights for next cycle
            ToneRegistry.RecoverWeights();

            return result;
        }

        // --- Helpers ---
        private static string ResolveCategory(Mood mood)
        {
            if (Enum.TryParse<MoodType>(mood.ToString(), out var moodType))
            {
                return ToneRegistry.ResolveCategory(moodType);
            }
            return "Neutral";
        }

        private static Tone Blend(Tone baseTone, Tone modifierTone, float weight = 0.5f)
        {
            var baseValue = (int)baseTone;
            var modifierValue = (int)modifierTone;
            var blendedValue = (int)(baseValue + (modifierValue - baseValue) * weight);
            return (Tone)blendedValue;
        }

        private static Tone ApplyBias(Tone candidate, Tone preferred, float biasStrength, BiasMode mode, Random rng)
        {
            if (mode == BiasMode.Probabilistic)
                return rng.NextDouble() < biasStrength ? preferred : candidate;

            var candidateValue = (int)candidate;
            var preferredValue = (int)preferred;
            var adjustedValue = (int)(candidateValue + (preferredValue - candidateValue) * biasStrength);
            return (Tone)adjustedValue;
        }

        private static Tone BiasPreferredTone(VectorBias bias)
        {
            if (bias == null)
                return Tone.Neutral;

            var moodType = MoodTypeExtensions.Resolve(bias.MoodAxis);
            var category = ToneRegistry.ResolveCategory(moodType);

            var tones = ToneRegistry.GetByCategory(category).ToList();
            return tones.Any() ? ToneRegistry.SelectWeighted(category) : Tone.Neutral;
        }

        // --- IResolver implementation ---
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            // Adapt interface inputs into ResolveTone pipeline
            var moodTuple = (mv, new List<(Mood mood, Tone tone)>(), Mood.Neutral);
            var complement = Mood.Neutral;

            var toneResult = ResolveTone(
                moodTuple,
                complement,
                vb,
                BiasMode.Deterministic
            );

            // Build DeltaSummary from bias context
            var summary = new DeltaSummary
            {
                DeltaAxis   = vb.MoodAxis,
                Magnitude   = vb.Persistence,
                Hypotenuse  = (float)Math.Sqrt(vb.MoodAxis * vb.MoodAxis + vb.Persistence * vb.Persistence),
                Area        = vb.MoodAxis * vb.Persistence,
                AngleTheta  = (float)Math.Atan2(vb.Persistence, vb.MoodAxis),
                SinTheta    = (float)Math.Sin(vb.MoodAxis),
                CosTheta    = (float)Math.Cos(vb.MoodAxis),
                TanTheta    = (float)Math.Tan(vb.MoodAxis)
            };

            return new ResolutionResult(vb, summary);
        }
    }
}
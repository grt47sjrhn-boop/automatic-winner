using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.Registries.Lookups;
using substrate_shared.types.models;
using substrate_shared.types.models.Maps;
using substrate_shared.types.structs;

namespace substrate_shared.Registries
{
    public static class ToneRegistry
    {
        internal static readonly Dictionary<Tone, List<NarrativeTone>> _tones = new()
        {
            // Despair & Suffering
            { Tone.Wretched, [new NarrativeTone("Wretched", "Despair", "Negative")] },
            { Tone.Tragic, [new NarrativeTone("Tragic", "Despair", "Negative")] },
            { Tone.Despairing, [new NarrativeTone("Despairing", "Despair", "Negative")] },
            { Tone.Sad, [new NarrativeTone("Sad", "Despair", "Negative")] },
            { Tone.Melancholy, [new NarrativeTone("Melancholy", "Despair", "Negative")] },
            { Tone.Regretful, [new NarrativeTone("Regretful", "Despair", "Negative")] },
            { Tone.Pessimistic, [new NarrativeTone("Pessimistic", "Despair", "Negative")] },

            // Anxiety & Tension
            { Tone.Uneasy, [new NarrativeTone("Uneasy", "Anxiety", "Negative")] },
            { Tone.Tense, [new NarrativeTone("Tense", "Anxiety", "Negative")] },
            { Tone.Suspenseful, [new NarrativeTone("Suspenseful", "Anxiety", "Negative")] },
            { Tone.Stressed, [new NarrativeTone("Stressed", "Anxiety", "Negative")] },
            { Tone.Anxious, [new NarrativeTone("Anxious", "Anxiety", "Negative")] },
            { Tone.Apprehensive, [new NarrativeTone("Apprehensive", "Anxiety", "Negative")] },

            // Hostility & Anger
            { Tone.Angry, [new NarrativeTone("Angry", "Hostility", "Negative")] },
            { Tone.Hostile, [new NarrativeTone("Hostile", "Hostility", "Negative")] },
            { Tone.Bitter, [new NarrativeTone("Bitter", "Hostility", "Negative")] },
            { Tone.Cynical, [new NarrativeTone("Cynical", "Hostility", "Negative")] },
            { Tone.Confused, [new NarrativeTone("Confused", "Hostility", "Negative")] },
            { Tone.Disappointed, [new NarrativeTone("Disappointed", "Hostility", "Negative")] },

            // Dark & Solemn
            { Tone.Somber, [new NarrativeTone("Somber", "Darkness", "Negative")] },
            { Tone.Solemn, [new NarrativeTone("Solemn", "Darkness", "Negative")] },
            { Tone.Ominous, [new NarrativeTone("Ominous", "Darkness", "Negative")] },
            { Tone.Dark, [new NarrativeTone("Dark", "Darkness", "Negative")] },
            { Tone.Detached, [new NarrativeTone("Detached", "Darkness", "Negative")] },

            // Neutral group
            { Tone.Neutral, [new NarrativeTone("Neutral", "Neutral", "Neutral")] },
            {
                Tone.Nostalgic, [
                    new NarrativeTone("Nostalgic", "Neutral", "Neutral"),
                    new NarrativeTone("Nostalgic (Positive)", "Neutral", "Positive"),
                    new NarrativeTone("Nostalgic (Negative)", "Neutral", "Negative")
                ]
            },

            // Joy & Celebration
            { Tone.Joyful, [new NarrativeTone("Joyful", "Joy", "Positive")] },
            { Tone.Celebratory, [new NarrativeTone("Celebratory", "Joy", "Positive")] },
            { Tone.Excited, [new NarrativeTone("Excited", "Joy", "Positive")] },
            { Tone.Lively, [new NarrativeTone("Lively", "Joy", "Positive")] },
            { Tone.Uplifting, [new NarrativeTone("Uplifting", "Joy", "Positive")] },
            { Tone.Surprised, [new NarrativeTone("Surprised", "Joy", "Positive")] },

            // Love & Affection
            { Tone.Adoring, [new NarrativeTone("Adoring", "Love", "Positive")] },
            { Tone.Affectionate, [new NarrativeTone("Affectionate", "Love", "Positive")] },
            { Tone.Romantic, [new NarrativeTone("Romantic", "Love", "Positive")] },
            { Tone.Friendly, [new NarrativeTone("Friendly", "Love", "Positive")] },
            { Tone.Comforting, [new NarrativeTone("Comforting", "Love", "Positive")] },
            { Tone.Compassionate, [new NarrativeTone("Compassionate", "Love", "Positive")] },
            { Tone.Empathetic, [new NarrativeTone("Empathetic", "Love", "Positive")] },

            // Confidence & Hope
            { Tone.Confident, [new NarrativeTone("Confident", "Confidence", "Positive")] },
            { Tone.Hopeful, [new NarrativeTone("Hopeful", "Confidence", "Positive")] },
            { Tone.Optimistic, [new NarrativeTone("Optimistic", "Confidence", "Positive")] },
            { Tone.Grateful, [new NarrativeTone("Grateful", "Confidence", "Positive")] },
            { Tone.Encouraging, [new NarrativeTone("Encouraging", "Confidence", "Positive")] },
            { Tone.Approving, [new NarrativeTone("Approving", "Confidence", "Positive")] },

            // Wonder & Serenity
            { Tone.Amused, [new NarrativeTone("Amused", "Wonder", "Positive")] },
            { Tone.Playful, [new NarrativeTone("Playful", "Wonder", "Positive")] },
            { Tone.Awestruck, [new NarrativeTone("Awestruck", "Wonder", "Positive")] },
            { Tone.Serene, [new NarrativeTone("Serene", "Wonder", "Positive")] },
            { Tone.Idyllic, [new NarrativeTone("Idyllic", "Wonder", "Positive")] },
            { Tone.Ecstatic, [new NarrativeTone("Ecstatic", "Wonder", "Positive")] }
        };

        // --- Angular slice definitions: each slice maps to a category label
        internal static readonly List<(float min, float max, string category)> _angularCategories = new()
        {
            (-MathF.PI, -3 * MathF.PI / 4f, "Despair"),
            (-3 * MathF.PI / 4f, -MathF.PI / 2f, "Hostility"),
            (-MathF.PI / 2f, -MathF.PI / 4f, "Darkness"),
            (-MathF.PI / 4f, MathF.PI / 4f, "Neutral"),
            (MathF.PI / 4f, MathF.PI / 2f, "Anxiety"),
            (MathF.PI / 2f, 3 * MathF.PI / 4f, "Resonance"),
            (3 * MathF.PI / 4f, MathF.PI, "Joy")
        };
        
        // Weights keyed by NarrativeTone
        internal static readonly Dictionary<NarrativeTone, float> _weights =
            _tones.Values.SelectMany(list => list)
                .Where(nt => nt != null)
                .ToDictionary(nt => nt, nt => 1f);

        


        // --- Bias mapping ---
        public static BiasMap PopulateBiasMap(float theta, float axis, float persistenceValue, float erosionFactor, float direction)
        {
            var categories = _angularCategories.Select(ac => ac.category).Distinct();
            var map = new BiasMap(categories);

            // 1. Angle → category bias
            var category = ToneRegistryLookups.ResolveCategoryFromAngle(theta);
            map.AddBias(category, 1.0f);

            // 2. Axis → directional bias
            var axisBias = Math.Clamp(axis / 10f, -1f, 1f);
            if (axisBias > 0) map.AddBias("Confidence", axisBias, "Positive");
            else if (axisBias < 0) map.AddBias("Despair", Math.Abs(axisBias), "Negative");

            // 3. Persistence → personality bias
            var persistenceBias = Math.Clamp(persistenceValue / 10f, 0f, 1f);
            var tempered = persistenceBias * (1f - erosionFactor);
            map.AddBias(category, tempered);

            // 4. Direction → nudge
            if (direction > 0) map.AddBias("Confidence", direction, "Positive");
            else if (direction < 0) map.AddBias("Despair", Math.Abs(direction), "Negative");

            return map;
        }
    }
}
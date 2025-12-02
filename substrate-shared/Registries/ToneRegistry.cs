using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.types.models;

namespace substrate_shared.Registries
{
    public static class ToneRegistry
    {
        // --- Tone definitions ---
        private static readonly Dictionary<Tone, List<NarrativeTone>> _tones = new()
        {
            // Despair & Suffering
            { Tone.Wretched,     [new NarrativeTone("Wretched", "Despair", "Negative")] },
            { Tone.Tragic,       [new NarrativeTone("Tragic", "Despair", "Negative")] },
            { Tone.Despairing,   [new NarrativeTone("Despairing", "Despair", "Negative")] },
            { Tone.Sad,          [new NarrativeTone("Sad", "Despair", "Negative")] },
            { Tone.Melancholy,   [new NarrativeTone("Melancholy", "Despair", "Negative")] },
            { Tone.Regretful,    [new NarrativeTone("Regretful", "Despair", "Negative")] },
            { Tone.Pessimistic,  [new NarrativeTone("Pessimistic", "Despair", "Negative")] },

            // Anxiety & Tension
            { Tone.Uneasy,       [new NarrativeTone("Uneasy", "Anxiety", "Negative")] },
            { Tone.Tense,        [new NarrativeTone("Tense", "Anxiety", "Negative")] },
            { Tone.Suspenseful,  [new NarrativeTone("Suspenseful", "Anxiety", "Negative")] },
            { Tone.Stressed,     [new NarrativeTone("Stressed", "Anxiety", "Negative")] },
            { Tone.Anxious,      [new NarrativeTone("Anxious", "Anxiety", "Negative")] },
            { Tone.Apprehensive, [new NarrativeTone("Apprehensive", "Anxiety", "Negative")] },

            // Hostility & Anger
            { Tone.Angry,        [new NarrativeTone("Angry", "Hostility", "Negative")] },
            { Tone.Hostile,      [new NarrativeTone("Hostile", "Hostility", "Negative")] },
            { Tone.Bitter,       [new NarrativeTone("Bitter", "Hostility", "Negative")] },
            { Tone.Cynical,      [new NarrativeTone("Cynical", "Hostility", "Negative")] },
            { Tone.Confused,     [new NarrativeTone("Confused", "Hostility", "Negative")] },
            { Tone.Disappointed, [new NarrativeTone("Disappointed", "Hostility", "Negative")] },

            // Dark & Solemn
            { Tone.Somber,       [new NarrativeTone("Somber", "Darkness", "Negative")] },
            { Tone.Solemn,       [new NarrativeTone("Solemn", "Darkness", "Negative")] },
            { Tone.Ominous,      [new NarrativeTone("Ominous", "Darkness", "Negative")] },
            { Tone.Dark,         [new NarrativeTone("Dark", "Darkness", "Negative")] },
            { Tone.Detached,     [new NarrativeTone("Detached", "Darkness", "Negative")] },

            // Neutral group
            { Tone.Neutral,      [new NarrativeTone("Neutral", "Neutral", "Neutral")] },
            { Tone.Nostalgic,    [
                new NarrativeTone("Nostalgic", "Neutral", "Neutral"),
                new NarrativeTone("Nostalgic (Positive)", "Neutral", "Positive"),
                new NarrativeTone("Nostalgic (Negative)", "Neutral", "Negative")
            ]},

            // Joy & Celebration
            { Tone.Joyful,       [new NarrativeTone("Joyful", "Joy", "Positive")] },
            { Tone.Celebratory,  [new NarrativeTone("Celebratory", "Joy", "Positive")] },
            { Tone.Excited,      [new NarrativeTone("Excited", "Joy", "Positive")] },
            { Tone.Lively,       [new NarrativeTone("Lively", "Joy", "Positive")] },
            { Tone.Uplifting,    [new NarrativeTone("Uplifting", "Joy", "Positive")] },
            { Tone.Surprised,    [new NarrativeTone("Surprised", "Joy", "Positive")] },

            // Love & Affection
            { Tone.Adoring,      [new NarrativeTone("Adoring", "Love", "Positive")] },
            { Tone.Affectionate, [new NarrativeTone("Affectionate", "Love", "Positive")] },
            { Tone.Romantic,     [new NarrativeTone("Romantic", "Love", "Positive")] },
            { Tone.Friendly,     [new NarrativeTone("Friendly", "Love", "Positive")] },
            { Tone.Comforting,   [new NarrativeTone("Comforting", "Love", "Positive")] },
            { Tone.Compassionate,[new NarrativeTone("Compassionate", "Love", "Positive")] },
            { Tone.Empathetic,   [new NarrativeTone("Empathetic", "Love", "Positive")] },

            // Confidence & Hope
            { Tone.Confident,    [new NarrativeTone("Confident", "Confidence", "Positive")] },
            { Tone.Hopeful,      [new NarrativeTone("Hopeful", "Confidence", "Positive")] },
            { Tone.Optimistic,   [new NarrativeTone("Optimistic", "Confidence", "Positive")] },
            { Tone.Grateful,     [new NarrativeTone("Grateful", "Confidence", "Positive")] },
            { Tone.Encouraging,  [new NarrativeTone("Encouraging", "Confidence", "Positive")] },
            { Tone.Approving,    [new NarrativeTone("Approving", "Confidence", "Positive")] },

            // Wonder & Serenity
            { Tone.Amused,       [new NarrativeTone("Amused", "Wonder", "Positive")] },
            { Tone.Playful,      [new NarrativeTone("Playful", "Wonder", "Positive")] },
            { Tone.Awestruck,    [new NarrativeTone("Awestruck", "Wonder", "Positive")] },
            { Tone.Serene,       [new NarrativeTone("Serene", "Wonder", "Positive")] },
            { Tone.Idyllic,      [new NarrativeTone("Idyllic", "Wonder", "Positive")] },
            { Tone.Ecstatic,     [new NarrativeTone("Ecstatic", "Wonder", "Positive")] }
        };

        // --- NarrativeGroup → Category mapping ---
        private static readonly Dictionary<string, string> NarrativeGroupToCategory = new()
        {
            { "Abyssal States", "Despair" },
            { "Shadow States", "Despair" },
            { "Anxious Currents", "Anxiety" },
            { "Fractured Tensions", "Anxiety" },
            { "Equilibrium", "Neutral" },
            { "Gentle Radiance", "Joy" },
            { "Bright Currents", "Joy" },
            { "Core Joy", "Joy" },
            { "Rising Hope", "Confidence" },
            { "Transcendent Peaks", "Wonder" }
        };

        // --- Runtime weights ---
        private static readonly Dictionary<Tone, float> _weights = _tones.Keys.ToDictionary(t => t, t => 1f);
        private static readonly Random _rng = new();

        public static IEnumerable<Tone> All => _tones.Keys;

        public static IEnumerable<NarrativeTone> Get(Tone tone) =>
            _tones.TryGetValue(tone, out var list)
                ? list
                : new[] { new NarrativeTone(tone.ToString(), "Neutral", "Neutral") };

        public static bool TryParse(string label, out Tone tone)
        {
            foreach (var kvp in _tones)
            {
                if (kvp.Value.Any(nt => nt != null &&
                                        nt.Label.Equals(label, StringComparison.OrdinalIgnoreCase)))
                {
                    tone = kvp.Key;
                    return true;
                }
            }
            tone = default;
            return false;
        }
        
        // --- Category resolution ---
        public static string ResolveCategory(MoodType moodType)
        {
            var group = moodType.GetNarrativeGroup();
            return NarrativeGroupToCategory.TryGetValue(group, out var category)
                ? category
                : "Neutral";
        }

        public static IEnumerable<Tone> GetByCategory(string category) =>
            _tones.Where(kvp => kvp.Value.Any(nt => nt != null &&
                                                    nt.Category.Equals(category, StringComparison.OrdinalIgnoreCase)))
                .Select(kvp => kvp.Key);


        // --- Weighted selection ---
        public static Tone SelectWeighted(string category)
        {
            var candidates = GetByCategory(category).ToList();
            if (!candidates.Any())
                return Tone.Neutral;

            float total = candidates.Sum(t => _weights[t]);
            if (total <= 0)
            {
                foreach (var tone in candidates)
                    _weights[tone] = 1f;
                total = candidates.Sum(t => _weights[t]);
            }

            float roll = (float)_rng.NextDouble() * total;
            foreach (var tone in candidates)
            {
                roll -= _weights[tone];
                if (roll <= 0)
                {
                    _weights[tone] = Math.Max(0.1f, _weights[tone] * 0.25f);
                    return tone;
                }
            }

            return candidates.First(); // fallback
        }

        /// <summary>
        /// Gradually recover weights so tones re-enter rotation.
        /// </summary>
        public static void RecoverWeights(float step = 0.1f)
        {
            foreach (var tone in _weights.Keys.ToList())
            {
                _weights[tone] = Math.Min(1f, _weights[tone] + step);
            }
        }

        /// <summary>
        /// Convenience: resolve directly from MoodType to a weighted Tone.
        /// </summary>
        public static Tone ResolveTone(MoodType moodType)
        {
            var category = ResolveCategory(moodType);
            return SelectWeighted(category);
        }

        public static void AuditTones()
        {
            foreach (var kvp in _tones)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value[i] == null)
                    {
                        Console.WriteLine($"Tone {kvp.Key} has a null NarrativeTone at index {i}!");
                    }
                }
            }
        }

    }
}
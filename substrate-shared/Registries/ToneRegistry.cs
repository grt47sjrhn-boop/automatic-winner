using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.Registries.Lookups;
using substrate_shared.Registries.Tools;
using substrate_shared.types.models;
using substrate_shared.types.models.Maps;

namespace substrate_shared.Registries
{
    public static class ToneRegistry
    {
        // --- Seeded tones -----------------------------------------------------
        internal static readonly Dictionary<Tone, List<NarrativeTone>> _tones = new()
        {
            // Despair & Suffering
            { Tone.Wretched,       [new NarrativeTone("Wretched", "Despair", "Negative")] },
            { Tone.Tragic,         [new NarrativeTone("Tragic", "Despair", "Negative")] },
            { Tone.Despairing,     [new NarrativeTone("Despairing", "Despair", "Negative")] },
            { Tone.Sad,            [new NarrativeTone("Sad", "Despair", "Negative")] },
            { Tone.Melancholy,     [new NarrativeTone("Melancholy", "Despair", "Negative")] },
            { Tone.Regretful,      [new NarrativeTone("Regretful", "Despair", "Negative")] },
            { Tone.Pessimistic,    [new NarrativeTone("Pessimistic", "Despair", "Negative")] },

            // Anxiety & Tension
            { Tone.Uneasy,         [new NarrativeTone("Uneasy", "Anxiety", "Negative")] },
            { Tone.Tense,          [new NarrativeTone("Tense", "Anxiety", "Negative")] },
            { Tone.Suspenseful,    [new NarrativeTone("Suspenseful", "Anxiety", "Negative")] },
            { Tone.Stressed,       [new NarrativeTone("Stressed", "Anxiety", "Negative")] },
            { Tone.Anxious,        [new NarrativeTone("Anxious", "Anxiety", "Negative")] },
            { Tone.Apprehensive,   [new NarrativeTone("Apprehensive", "Anxiety", "Negative")] },

            // Hostility & Anger
            { Tone.Angry,          [new NarrativeTone("Angry", "Hostility", "Negative")] },
            { Tone.Hostile,        [new NarrativeTone("Hostile", "Hostility", "Negative")] },
            { Tone.Bitter,         [new NarrativeTone("Bitter", "Hostility", "Negative")] },
            { Tone.Cynical,        [new NarrativeTone("Cynical", "Hostility", "Negative")] },
            { Tone.Confused,       [new NarrativeTone("Confused", "Hostility", "Negative")] },
            { Tone.Disappointed,   [new NarrativeTone("Disappointed", "Hostility", "Negative")] },

            // Dark & Solemn
            { Tone.Somber,         [new NarrativeTone("Somber", "Darkness", "Negative")] },
            { Tone.Solemn,         [new NarrativeTone("Solemn", "Darkness", "Negative")] },
            { Tone.Ominous,        [new NarrativeTone("Ominous", "Darkness", "Negative")] },
            { Tone.Dark,           [new NarrativeTone("Dark", "Darkness", "Negative")] },
            { Tone.Detached,       [new NarrativeTone("Detached", "Darkness", "Negative")] },

            // Neutral group
            { Tone.Neutral,        [new NarrativeTone("Neutral", "Neutral", "Neutral")] },
            {
                Tone.Nostalgic, [
                    new NarrativeTone("Nostalgic", "Neutral", "Neutral"),
                    new NarrativeTone("Nostalgic (Positive)", "Neutral", "Positive"),
                    new NarrativeTone("Nostalgic (Negative)", "Neutral", "Negative")
                ]
            },

            // Joy & Celebration
            { Tone.Joyful,         [new NarrativeTone("Joyful", "Joy", "Positive")] },
            { Tone.Celebratory,    [new NarrativeTone("Celebratory", "Joy", "Positive")] },
            { Tone.Excited,        [new NarrativeTone("Excited", "Joy", "Positive")] },
            { Tone.Lively,         [new NarrativeTone("Lively", "Joy", "Positive")] },
            { Tone.Uplifting,      [new NarrativeTone("Uplifting", "Joy", "Positive")] },
            { Tone.Surprised,      [new NarrativeTone("Surprised", "Joy", "Positive")] },

            // Love & Affection
            { Tone.Adoring,        [new NarrativeTone("Adoring", "Love", "Positive")] },
            { Tone.Affectionate,   [new NarrativeTone("Affectionate", "Love", "Positive")] },
            { Tone.Romantic,       [new NarrativeTone("Romantic", "Love", "Positive")] },
            { Tone.Friendly,       [new NarrativeTone("Friendly", "Love", "Positive")] },
            { Tone.Comforting,     [new NarrativeTone("Comforting", "Love", "Positive")] },
            { Tone.Compassionate,  [new NarrativeTone("Compassionate", "Love", "Positive")] },
            { Tone.Empathetic,     [new NarrativeTone("Empathetic", "Love", "Positive")] },

            // Confidence & Hope
            { Tone.Confident,      [new NarrativeTone("Confident", "Confidence", "Positive")] },
            { Tone.Hopeful,        [new NarrativeTone("Hopeful", "Confidence", "Positive")] },
            { Tone.Optimistic,     [new NarrativeTone("Optimistic", "Confidence", "Positive")] },
            { Tone.Grateful,       [new NarrativeTone("Grateful", "Confidence", "Positive")] },
            { Tone.Encouraging,    [new NarrativeTone("Encouraging", "Confidence", "Positive")] },
            { Tone.Approving,      [new NarrativeTone("Approving", "Confidence", "Positive")] },

            // Wonder & Serenity
            { Tone.Amused,         [new NarrativeTone("Amused", "Wonder", "Positive")] },
            { Tone.Playful,        [new NarrativeTone("Playful", "Wonder", "Positive")] },
            { Tone.Awestruck,      [new NarrativeTone("Awestruck", "Wonder", "Positive")] },
            { Tone.Serene,         [new NarrativeTone("Serene", "Wonder", "Positive")] },
            { Tone.Idyllic,        [new NarrativeTone("Idyllic", "Wonder", "Positive")] },
            { Tone.Ecstatic,       [new NarrativeTone("Ecstatic", "Wonder", "Positive")] },

            // Intensity
            { Tone.Intense,        [new NarrativeTone("Intense", "Intensity", "Neutral")] },
            { Tone.Forceful,       [new NarrativeTone("Forceful", "Intensity", "Neutral")] },

            // Light
            { Tone.Bright,         [new NarrativeTone("Bright", "Light", "Positive")] },
            { Tone.Radiant,        [new NarrativeTone("Radiant", "Light", "Positive")] },
            { Tone.Luminous,       [new NarrativeTone("Luminous", "Light", "Positive")] },

            // Resonance
            { Tone.Harmonic,       [new NarrativeTone("Harmonic", "Resonance", "Positive")] },
            { Tone.Resounding,     [new NarrativeTone("Resounding", "Resonance", "Positive")] },
            { Tone.Melodic,        [new NarrativeTone("Melodic", "Resonance", "Positive")] },

            // Calm
            { Tone.Calm,           [new NarrativeTone("Calm", "Calm", "Neutral")] },
        };

        // --- Angular wheel ----------------------------------------------------
        internal static readonly List<(float min, float max, string category)> _angularCategories =
        [
            (-MathF.PI / 6f, MathF.PI / 6f, "Neutral"),
            (MathF.PI - MathF.PI / 6f, MathF.PI + MathF.PI / 6f, "Intensity"),
            (MathF.PI / 3f, 2 * MathF.PI / 3f, "Joy"),
            (-2 * MathF.PI / 3f, -MathF.PI / 3f, "Despair"),
            (MathF.PI / 6f, MathF.PI / 3f, "Calm"),
            (-MathF.PI / 3f, -MathF.PI / 6f, "Anxiety"),
            (2 * MathF.PI / 3f, 5 * MathF.PI / 6f, "Light"),
            (-5 * MathF.PI / 6f, -2 * MathF.PI / 3f, "Darkness"),
            (23 * MathF.PI / 24f, 25 * MathF.PI / 24f, "Resonance"),
            (-25 * MathF.PI / 24f, -23 * MathF.PI / 24f, "Hostility")
        ];

        // --- Weights ----------------------------------------------------------
        internal static readonly Dictionary<NarrativeTone, float> _weights =
            _tones.Values.SelectMany(list => list)
                .Where(nt => nt != null)
                .ToDictionary(nt => nt, nt => 1f);

        // --- Bridge lookup (fixes Neutral collapse) ---------------------------
        public static List<NarrativeTone> GetTonesByCategory(string category)
        {
            var normalized = category?.Trim();
            if (string.IsNullOrEmpty(normalized))
                return GetNeutralTones();

            // 1) Direct category match
            var direct = _tones.Values
                .SelectMany(list => list)
                .Where(t => t != null && t.Category.Equals(normalized, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (direct.Count > 0) return direct;

            // 2) Opposite category fallback
            var opposite = ToneRegistryDiagnostics.GetOpposite(normalized);
            var oppTones = _tones.Values
                .SelectMany(list => list)
                .Where(t => t != null && t.Category.Equals(opposite, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (oppTones.Count > 0)
            {
                ToneRegistryDiagnostics.LogCandidateResolution("Fallback", normalized, opposite);
                return oppTones;
            }

            // 3) Final fallback: Neutral
            return GetNeutralTones();
        }

        private static List<NarrativeTone> GetNeutralTones()
        {
            return _tones.Values
                .SelectMany(list => list)
                .Where(t => t != null && t.Category.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }


        // --- Bias mapping ------------------------------------------------------
        public static BiasMap PopulateBiasMap(
            float theta,
            float axis,
            float persistenceValue,
            float erosionFactor,
            float direction)
        {
            var categories = _angularCategories.Select(ac => ac.category).Distinct();
            var map = new BiasMap(categories);

            // 0) Base epsilon to avoid zero-weight collapse
            const float epsilon = 0.01f;

            // 1) Angle → category bias
            var category = ToneRegistryLookups.ResolveCategoryFromAngle(theta);
            map.AddBias(category, 1.0f, "Angle");

            ToneRegistryDiagnostics.ValidateOpposites(_angularCategories);

            // 2) Axis → directional bias (Confidence vs Despair)
            ToneRegistryLookups.ResolveAxisInfluence(map, axis);

            var axisBias = Math.Clamp(axis / 10f, -1f, 1f);
            if (axisBias > 0)
                ToneRegistryDiagnostics.LogCandidateResolution("Axis", "Confidence", "Confidence");
            else if (axisBias < 0)
                ToneRegistryDiagnostics.LogCandidateResolution("Axis", "Despair", "Despair");

            // 3) Persistence → personality bias (tempered by erosion)
            var persistenceBias = Math.Clamp(persistenceValue / 10f, 0f, 1f);
            var tempered = persistenceBias * Math.Max(0f, (1f - erosionFactor));
            if (tempered > epsilon)
                map.AddBias(category, tempered, "Persistence");

            var resolvedPersistence = ToneRegistryDiagnostics.ResolvePersistence(category, category);
            ToneRegistryDiagnostics.LogCandidateResolution("Persistence", category, resolvedPersistence);

            // 4) Direction → nudge (Confidence/Despair)
            if (direction > epsilon)
            {
                map.AddBias("Confidence", direction, "Direction");
                ToneRegistryDiagnostics.LogCandidateResolution("Direction", "Confidence", "Confidence");
            }
            else if (direction < -epsilon)
            {
                map.AddBias("Despair", Math.Abs(direction), "Direction");
                ToneRegistryDiagnostics.LogCandidateResolution("Direction", "Despair", "Despair");
            }

            // 5) Optional angular smoothing: bleed into adjacent categories
            var adjacents = ToneRegistryLookups.GetAdjacentCategories(category);
            var bleed = 0.25f; // 25% of primary angle bias
            foreach (var adj in adjacents)
                map.AddBias(adj, bleed, "AdjacencyBleed");

            // 6) Normalize for stability
            map.Normalize(epsilon);

            return map;
        }

        // --- Audit helpers -----------------------------------------------------
        public static void AuditCategoryCoverage()
        {
            Console.WriteLine("=== ToneRegistry Audit ===");

            var wheel = _angularCategories.Select(c => c.category).Distinct().ToHashSet(StringComparer.OrdinalIgnoreCase);
            var seeded = _tones.Values.SelectMany(v => v).Select(t => t.Category).Distinct().ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingInSeed = wheel.Except(seeded).ToList();
            var overlaysNotInWheel = seeded.Except(wheel).ToList();

            if (missingInSeed.Count == 0)
                Console.WriteLine("[OK] All wheel categories have seeded tones.");
            else
                Console.WriteLine("[WARN] Wheel categories missing tones: " + string.Join(", ", missingInSeed));

            if (overlaysNotInWheel.Count > 0)
                Console.WriteLine("[INFO] Overlay categories (seeded but not in wheel): " + string.Join(", ", overlaysNotInWheel));

            Console.WriteLine("=== End Audit ===");
        }
    }
}
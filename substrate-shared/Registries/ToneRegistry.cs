using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.types.models;
using substrate_shared.types.models.Maps;
using substrate_shared.types.structs;

namespace substrate_shared.Registries
{
    public static class ToneRegistry
    {
        // --- Tone definitions ---
        private static readonly Dictionary<Tone, List<NarrativeTone>> _tones = new()
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

        // Angular slice definitions: each slice maps to a category label
        private static readonly List<(float min, float max, string category)> _angularCategories = new()
        {
            (-MathF.PI, -3 * MathF.PI / 4f, "Despair"),
            (-3 * MathF.PI / 4f, -MathF.PI / 2f, "Hostility"),
            (-MathF.PI / 2f, -MathF.PI / 4f, "Darkness"),
            (-MathF.PI / 4f, MathF.PI / 4f, "Neutral"),
            (MathF.PI / 4f, MathF.PI / 2f, "Anxiety"),
            (MathF.PI / 2f, 3 * MathF.PI / 4f, "Resonance"),
            (3 * MathF.PI / 4f, MathF.PI, "Joy")
        };

        private static readonly Dictionary<Tone, float> _weights = _tones.Keys.ToDictionary(t => t, t => 1f);
        private static readonly Random _rng = new();

        public static BiasMap PopulateBiasMap(float theta, float axis, float persistenceValue, float erosionFactor,
            float direction)
        {
            var categories = _angularCategories.Select(ac => ac.category).Distinct();
            var map = new BiasMap(categories);

            // 1. Angle → category bias
            var category = ResolveCategoryFromAngle(theta);
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

        public static string ResolveCategoryFromAngle(float theta)
        {
            while (theta > MathF.PI) theta -= 2 * MathF.PI;
            while (theta < -MathF.PI) theta += 2 * MathF.PI;

            foreach (var (min, max, category) in _angularCategories)
            {
                if (theta >= min && theta < max)
                    return category;
            }

            return "Neutral";
        }

        public static Tone ResolveFromAngle(float theta)
        {
            while (theta > MathF.PI) theta -= 2 * MathF.PI;
            while (theta < -MathF.PI) theta += 2 * MathF.PI;

            foreach (var (min, max, category) in _angularCategories)
            {
                if (theta >= min && theta < max)
                    return SelectWeighted(category);
            }

            return Tone.Neutral;
        }

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

        public static IEnumerable<Tone> GetByCategory(string category)
        {
            return _tones.Where(kvp =>
                    kvp.Value.Any(nt =>
                        nt != null && nt.Category.Equals(category, StringComparison.OrdinalIgnoreCase)))
                .Select(kvp => kvp.Key);
        }

        /// <summary>
        /// Get all tones in the same category as the given tone.
        /// </summary>
        public static IEnumerable<Tone> GetNeighborhoodByTone(Tone tone)
        {
            if (!_tones.ContainsKey(tone) || !_tones[tone].Any())
                return Enumerable.Empty<Tone>();

            var category = _tones[tone].First().Category;
            return GetByCategory(category);
        }
        
        /// <summary>
        /// Get adjacent categories based on theta slices.
        /// </summary>
        public static List<string> GetAdjacentCategories(string category)
        {
            var index = _angularCategories.FindIndex(ac => ac.category.Equals(category, StringComparison.OrdinalIgnoreCase));
            if (index == -1) return new List<string>();

            var neighbors = new List<string>();
            if (index > 0) neighbors.Add(_angularCategories[index - 1].category);
            if (index < _angularCategories.Count - 1) neighbors.Add(_angularCategories[index + 1].category);
            return neighbors;
        }

        /// <summary>
        /// Get tones from categories adjacent to the given tone’s category.
        /// </summary>
        public static IEnumerable<Tone> GetAdjacentByTone(Tone tone)
        {
            if (!_tones.ContainsKey(tone) || !_tones[tone].Any())
                return Enumerable.Empty<Tone>();

            var category = _tones[tone].First().Category;
            var adjCats = GetAdjacentCategories(category);

            return adjCats.SelectMany(cat => GetByCategory(cat));
        }
        
        /// <summary>
        /// Get the complement category for a given tone, based on angular slices.
        /// </summary>
        public static string GetComplementCategory(Tone tone)
        {
            if (!_tones.ContainsKey(tone) || !_tones[tone].Any())
                return "Neutral";

            var category = _tones[tone].First().Category;

            // Find the slice for this category
            var slice = _angularCategories.FirstOrDefault(ac =>
                ac.category.Equals(category, StringComparison.OrdinalIgnoreCase));

            if (slice == default) return "Neutral";

            // Compute midpoint of slice
            var midpoint = (slice.min + slice.max) / 2f;

            // Rotate π radians to find opposite slice
            var complementTheta = midpoint + MathF.PI;
            while (complementTheta > MathF.PI) complementTheta -= 2 * MathF.PI;
            while (complementTheta < -MathF.PI) complementTheta += 2 * MathF.PI;

            // Resolve category at complement angle
            return ResolveCategoryFromAngle(complementTheta);
        }

        /// <summary>
        /// Get the complement tones for a given tone.
        /// </summary>
        public static IEnumerable<Tone> GetComplementNeighborhood(Tone tone)
        {
            var complementCategory = GetComplementCategory(tone);
            return GetByCategory(complementCategory);
        }

        
        public static void ResolveAxisInfluence(BiasMap map, float axis)
        {
            var axisBias = Math.Clamp(axis / 10f, -1f, 1f);

            switch (axisBias)
            {
                case > 0:
                    map.AddBias("Confidence", axisBias, "Positive");
                    break;
                case < 0:
                    map.AddBias("Despair", Math.Abs(axisBias), "Negative");
                    break;
                default:
                    map.AddBias("Neutral", 0.1f, "Neutral");
                    break;
            }
        }

        // Expose current weights for summaries/debugging
        public static IReadOnlyDictionary<Tone, float> CurrentWeights()
        {
            return _weights;
        }
        
        public static IEnumerable<AngularCategoryInfo> GetAngularCategories()
        {
            return _angularCategories.Select(ac => new AngularCategoryInfo
            {
                MinTheta = ac.min,
                MaxTheta = ac.max,
                Category = ac.category
            });
        }
        
        private static readonly Dictionary<string, TraitAffinity> _categoryAffinityMap = new()
        {
            { "Despair",   TraitAffinity.FracturedLegacy },
            { "Hostility", TraitAffinity.FracturedLegacy },
            { "Darkness",  TraitAffinity.FracturedLegacy },
            { "Neutral",   TraitAffinity.Equilibrium },
            { "Anxiety",   TraitAffinity.Inertia },
            { "Resonance", TraitAffinity.ResilientHarmony },
            { "Joy",       TraitAffinity.ResilientHarmony }
        };

        public static TraitAffinity ResolveAffinityFromCategory(string category)
        {
            return _categoryAffinityMap.GetValueOrDefault(category, TraitAffinity.None);
        }
    }
}
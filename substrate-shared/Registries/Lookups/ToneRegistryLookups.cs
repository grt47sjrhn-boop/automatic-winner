using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.types.models;
using substrate_shared.types.models.Maps;
using substrate_shared.types.structs;

namespace substrate_shared.Registries.Lookups
{
    public static class ToneRegistryLookups
    {
        private static readonly Random _rng = new();

        // --- Angle → Category --------------------------------------------------

        public static string ResolveCategoryFromAngle(float theta)
        {
            // Normalize to [0, 2π)
            while (theta < 0) theta += 2 * MathF.PI;
            while (theta >= 2 * MathF.PI) theta -= 2 * MathF.PI;

            foreach (var (min, max, category) in ToneRegistry._angularCategories)
            {
                if (theta >= min && theta < max)
                    return category;
            }

            return "Neutral";
        }

        // --- Angle → Tone (guarded by category) -------------------------------

        public static NarrativeTone ResolveFromAngle(float theta)
        {
            // Normalize to [0, 2π) and resolve category first
            while (theta < 0) theta += 2 * MathF.PI;
            while (theta >= 2 * MathF.PI) theta -= 2 * MathF.PI;

            var category = ResolveCategoryFromAngle(theta);

            // Try weighted selection
            var sampled = SelectWeighted(category);

            // Guard: ensure tone honors category; otherwise resolve strictly
            if (sampled == null || !string.Equals(sampled.Category, category, StringComparison.OrdinalIgnoreCase))
                sampled = ResolveFromCategory(category);

            return sampled ?? DefaultNeutralTone();
        }

        // --- Strict category resolution ---------------------------------------

        public static NarrativeTone ResolveFromCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return DefaultNeutralTone();

            var candidates = GetByCategory(category).ToList();
            if (candidates.Count == 0)
                return DefaultNeutralTone();

            // Prefer highest-weight tone if available; otherwise first
            var best = candidates
                .OrderByDescending(t => ToneRegistry._weights.TryGetValue(t, out var w) ? w : 1f)
                .First();

            return best;
        }

        // --- Weighted selection ------------------------------------------------

        public static NarrativeTone SelectWeighted(string category)
        {
            var candidates = GetByCategory(category).ToList();
            if (!candidates.Any())
                return DefaultNeutralTone();

            // Initialize missing weights
            foreach (var tone in candidates)
            {
                if (!ToneRegistry._weights.ContainsKey(tone))
                    ToneRegistry._weights[tone] = 1f;
            }

            float total = candidates.Sum(t => ToneRegistry._weights[t]);
            if (total <= 0)
            {
                foreach (var tone in candidates)
                    ToneRegistry._weights[tone] = 1f;
                total = candidates.Sum(t => ToneRegistry._weights[t]);
            }

            float roll = (float)_rng.NextDouble() * total;
            foreach (var tone in candidates)
            {
                roll -= ToneRegistry._weights[tone];
                if (roll <= 0)
                {
                    // Weight decay to avoid repeated selection dominance
                    ToneRegistry._weights[tone] = Math.Max(0.1f, ToneRegistry._weights[tone] * 0.25f);
                    return tone;
                }
            }

            return candidates.First(); // fallback
        }

        // Optional deterministic selection (useful for debug-stable traces)
        public static NarrativeTone SelectWeightedDeterministic(string category, int seed)
        {
            var candidates = GetByCategory(category).ToList();
            if (!candidates.Any())
                return DefaultNeutralTone();

            foreach (var tone in candidates)
            {
                if (!ToneRegistry._weights.ContainsKey(tone))
                    ToneRegistry._weights[tone] = 1f;
            }

            float total = candidates.Sum(t => ToneRegistry._weights[t]);
            if (total <= 0)
            {
                foreach (var tone in candidates)
                    ToneRegistry._weights[tone] = 1f;
                total = candidates.Sum(t => ToneRegistry._weights[t]);
            }

            var rng = new Random(seed);
            float roll = (float)rng.NextDouble() * total;

            foreach (var tone in candidates)
            {
                roll -= ToneRegistry._weights[tone];
                if (roll <= 0)
                {
                    return tone;
                }
            }

            return candidates.First();
        }

        // --- Category neighborhood --------------------------------------------

        public static IEnumerable<NarrativeTone> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return Enumerable.Empty<NarrativeTone>();

            return ToneRegistry._tones.Values
                .SelectMany(list => list ?? new List<NarrativeTone>())
                .Where(nt => nt != null &&
                             !string.IsNullOrEmpty(nt.Category) &&
                             nt.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all tones in the same category as the given tone.
        /// </summary>
        public static IEnumerable<NarrativeTone> GetNeighborhoodByTone(Tone tone)
        {
            if (!ToneRegistry._tones.ContainsKey(tone) || !ToneRegistry._tones[tone].Any())
                return Enumerable.Empty<NarrativeTone>();

            var category = ToneRegistry._tones[tone].First().Category;
            return GetByCategory(category);
        }

        /// <summary>
        /// Get adjacent categories based on theta slices.
        /// </summary>
        public static List<string> GetAdjacentCategories(string category)
        {
            var index = ToneRegistry._angularCategories.FindIndex(ac => ac.category.Equals(category, StringComparison.OrdinalIgnoreCase));
            if (index == -1) return new List<string>();

            var neighbors = new List<string>();
            if (index > 0) neighbors.Add(ToneRegistry._angularCategories[index - 1].category);
            if (index < ToneRegistry._angularCategories.Count - 1) neighbors.Add(ToneRegistry._angularCategories[index + 1].category);
            return neighbors;
        }

        public static IEnumerable<NarrativeTone> GetAdjacentByTone(Tone tone)
        {
            if (!ToneRegistry._tones.ContainsKey(tone) || !ToneRegistry._tones[tone].Any())
                return Enumerable.Empty<NarrativeTone>();

            var category = ToneRegistry._tones[tone].First().Category;
            var adjCats = GetAdjacentCategories(category);

            return adjCats.SelectMany(cat => GetByCategory(cat));
        }

        /// <summary>
        /// Get the complement category for a given tone, based on angular slices.
        /// </summary>
        public static string GetComplementCategory(Tone tone)
        {
            if (!ToneRegistry._tones.ContainsKey(tone) || !ToneRegistry._tones[tone].Any())
                return "Neutral";

            var category = ToneRegistry._tones[tone].First().Category;

            // Find the slice for this category
            var slice = ToneRegistry._angularCategories.FirstOrDefault(ac =>
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
        public static IEnumerable<NarrativeTone> GetComplementNeighborhood(Tone tone)
        {
            var complementCategory = GetComplementCategory(tone);
            return GetByCategory(complementCategory);
        }

        // --- Bias influence & diagnostics -------------------------------------

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
        public static IReadOnlyDictionary<NarrativeTone, float> CurrentWeights()
        {
            return ToneRegistry._weights;
        }

        public static IEnumerable<AngularCategoryInfo> GetAngularCategories()
        {
            return ToneRegistry._angularCategories.Select(ac => new AngularCategoryInfo
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

        public static void AuditToneRegistry()
        {
            Console.WriteLine("=== ToneRegistry Audit ===");

            foreach (var kvp in ToneRegistry._tones)
            {
                if (kvp.Value == null || !kvp.Value.Any())
                {
                    Console.WriteLine($"[ToneRegistry] Category {kvp.Key} has no tones.");
                }
                else if (kvp.Value.Any(nt => nt == null))
                {
                    Console.WriteLine($"[ToneRegistry] Category {kvp.Key} contains null entries.");
                }
                else
                {
                    Console.WriteLine($"[ToneRegistry] Category {kvp.Key} has {kvp.Value.Count} tones.");
                }
            }

            Console.WriteLine("=== End Audit ===");
        }

        public static string GetComplementCategoryFromString(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "Neutral";

            // Find the slice for this category
            var slice = ToneRegistry._angularCategories.FirstOrDefault(ac =>
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

        // --- Helpers -----------------------------------------------------------

        private static NarrativeTone DefaultNeutralTone()
        {
            return new NarrativeTone("Neutral", "Default", "Neutral") { Category = "Neutral" };
        }
        
        public static NarrativeTone SelectFromListWeighted(List<NarrativeTone> tones, string category)
        {
            if (tones == null || tones.Count == 0)
                return null;

            // Filter tones by category (safety guard)
            var candidates = tones
                .Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (candidates.Count == 0)
                candidates = tones; // fallback: use full list

            // Look up weights from ToneRegistry
            var weighted = candidates
                .Select(t => new
                {
                    Tone = t,
                    Weight = ToneRegistry._weights.TryGetValue(t, out var w) ? w : 1f
                })
                .ToList();

            // Normalize weights
            var total = weighted.Sum(x => x.Weight);
            if (total <= 0) return weighted.First().Tone;

            var rand = new Random();
            var roll = rand.NextDouble() * total;

            float cumulative = 0f;
            foreach (var entry in weighted)
            {
                cumulative += entry.Weight;
                if (roll <= cumulative)
                    return entry.Tone;
            }

            // Fallback: return first
            return weighted.First().Tone;
        }
    }
}
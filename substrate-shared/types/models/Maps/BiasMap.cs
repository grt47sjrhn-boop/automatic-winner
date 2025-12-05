using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace substrate_shared.types.models.Maps
{
    public class BiasMap
    {
        public Dictionary<string, float> GroupBiases { get; } = new();
        public Dictionary<string, float> PolarityBiases { get; } = new();

        // Default constructor: seed all known groups + polarities
        public BiasMap()
        {
            // Groups
            GroupBiases["Despair"]    = 0f;
            GroupBiases["Hostility"]  = 0f;
            GroupBiases["Darkness"]   = 0f;
            GroupBiases["Neutral"]    = 0f;
            GroupBiases["Anxiety"]    = 0f;
            GroupBiases["Resonance"]  = 0f;
            GroupBiases["Joy"]        = 0f;
            GroupBiases["Love"]       = 0f;
            GroupBiases["Confidence"] = 0f;
            GroupBiases["Wonder"]     = 0f;

            // Polarities
            PolarityBiases["Positive"] = 0f;
            PolarityBiases["Negative"] = 0f;
            PolarityBiases["Neutral"]  = 0f;
        }

        // Constructor that uses categories passed in
        public BiasMap(IEnumerable<string> categories)
        {
            foreach (var cat in categories)
            {
                GroupBiases.TryAdd(cat, 0f);
            }

            // Always seed polarities
            PolarityBiases["Positive"] = 0f;
            PolarityBiases["Negative"] = 0f;
            PolarityBiases["Neutral"]  = 0f;
        }

        // Helper to add bias safely to group + polarity
        public void AddBias(string category, float value, string? polarity = null)
        {
            if (!GroupBiases.TryAdd(category, value))
                GroupBiases[category] += value;

            if (string.IsNullOrEmpty(polarity)) return;
            
            if (!PolarityBiases.TryAdd(polarity, value))
                PolarityBiases[polarity] += value;
        }

        // Get the strongest bias group
        public string GetStrongestGroup()
        {
            return GroupBiases.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        // Get the strongest polarity
        public string GetStrongestPolarity()
        {
            return PolarityBiases.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        // Return the top N group biases as (Category, Weight) tuples
        public IEnumerable<(string Category, float Weight)> GetTopN(int topN)
        {
            if (topN <= 0)
                return [];

            return GroupBiases
                .OrderByDescending(kvp => kvp.Value)
                .Take(topN)
                .Select(kvp => (kvp.Key, kvp.Value));
        }

        public void Normalize(float epsilon)
        {
            // Guard: if no biases, nothing to normalize
            if (GroupBiases == null || GroupBiases.Count == 0)
                return;

            // 1. Clamp negatives and NaNs
            var keys = GroupBiases.Keys.ToList();
            foreach (var key in keys)
            {
                var value = GroupBiases[key];

                if (float.IsNaN(value) || float.IsInfinity(value))
                    value = 0f;

                if (value < 0f)
                    value = 0f;

                // Clamp tiny values up to epsilon
                if (value < epsilon)
                    value = epsilon;

                GroupBiases[key] = value;
            }

            // 2. Normalize so weights sum to 1.0
            var total = GroupBiases.Values.Sum();

            if (total <= 0f)
            {
                // If everything collapsed, assign uniform epsilon
                foreach (var key in keys)
                    GroupBiases[key] = epsilon;

                total = GroupBiases.Values.Sum();
            }

            // Scale all values proportionally
            foreach (var key in keys)
            {
                GroupBiases[key] /= total;
            }
        }
    }
}
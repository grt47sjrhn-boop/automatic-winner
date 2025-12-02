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
    }
}
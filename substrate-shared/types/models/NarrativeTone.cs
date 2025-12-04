using System;

namespace substrate_shared.types.models
{
    public class NarrativeTone
    {
        public string Label { get; }
        public string Category { get; set; } // logical category: Despair, Confidence, etc.
        public string BiasValue { get; }  // "Negative", "Positive", or "Neutral"
        public string Group { get; }      // optional grouping/display

        public NarrativeTone(string label, string category, string biasValue, string group = null)
        {
            Label = label ?? throw new ArgumentNullException(nameof(label));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            BiasValue = biasValue ?? throw new ArgumentNullException(nameof(biasValue));
            Group = group; // optional, can be null
        }

        public bool IsPositive => BiasValue.Equals("Positive", StringComparison.OrdinalIgnoreCase);
        public bool IsNegative => BiasValue.Equals("Negative", StringComparison.OrdinalIgnoreCase);
        public bool IsNeutral => BiasValue.Equals("Neutral", StringComparison.OrdinalIgnoreCase);
    }
}
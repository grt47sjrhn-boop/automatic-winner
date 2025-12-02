using System;

namespace substrate_shared.types.models
{
    public class NarrativeTone
    {
        public string Label { get; }
        public string Group { get; }
        public string BiasValue { get; } // "Negative", "Positive", or "Neutral"
        public string Category { get; set; }
        public NarrativeTone(string label, string group, string biasValue)
        {
            Label = label ?? throw new ArgumentNullException(nameof(label));
            Group = group ?? throw new ArgumentNullException(nameof(group));
            BiasValue = biasValue ?? throw new ArgumentNullException(nameof(biasValue));
        }

        public bool IsPositive => BiasValue.Equals("Positive", StringComparison.OrdinalIgnoreCase);
        public bool IsNegative => BiasValue.Equals("Negative", StringComparison.OrdinalIgnoreCase);
        public bool IsNeutral => BiasValue.Equals("Neutral", StringComparison.OrdinalIgnoreCase);
        
    }
}
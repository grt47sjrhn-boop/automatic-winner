using System;

namespace substrate_shared.types.models
{
    public class NarrativeTone(string label, string category, string biasValue, string? group = null)
    {
        public string Label { get; } = label ?? throw new ArgumentNullException(nameof(label));
        public string Category { get; set; } = category ?? throw new ArgumentNullException(nameof(category)); 
        public string BiasValue { get; } = biasValue ?? throw new ArgumentNullException(nameof(biasValue));

        public string? Group { get; } = group;

        public bool IsPositive => BiasValue.Equals("Positive", StringComparison.OrdinalIgnoreCase);
        public bool IsNegative => BiasValue.Equals("Negative", StringComparison.OrdinalIgnoreCase);
        public bool IsNeutral => BiasValue.Equals("Neutral", StringComparison.OrdinalIgnoreCase);
    }
}
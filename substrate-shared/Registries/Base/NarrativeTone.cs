using System;
using substrate_shared.Registries.enums;

namespace substrate_shared.Registries.Base
{
    public class NarrativeTone(string label, string category, Bias biasValue, string? group = null)
    {
        public string Label { get; } = label ?? throw new ArgumentNullException(nameof(label));
        public string Category { get; set; } = category ?? throw new ArgumentNullException(nameof(category));
        public Bias BiasValue { get; } = biasValue;
        
        public bool IsPositive => BiasValue == Bias.Positive;
        public bool IsNegative => BiasValue == Bias.Negative;
        public bool IsNeutral  => BiasValue == Bias.Neutral;
        
        public override string ToString() => $"{Label} ({Category}, {BiasValue})";
    }
}
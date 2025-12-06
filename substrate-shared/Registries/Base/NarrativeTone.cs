using substrate_shared.Registries.enums;

namespace substrate_shared.Registries.Base
{
    public class NarrativeTone
    {
        public ToneType Type { get; }       // Compact enum identifier
        public string Label { get; }        // Full atmospheric prose
        public string Category { get; }
        public Bias BiasValue { get; }
        public string? Group { get; }

        public int BiasMultiplier => BiasValue switch
        {
            Bias.Positive => +1,
            Bias.Negative => -1,
            Bias.Neutral  => 0,
            _             => 0
        };

        // Full constructor
        public NarrativeTone(ToneType type, string label, string category, Bias biasValue, string? group = null)
        {
            Type = type;
            Label = label;
            Category = category;
            BiasValue = biasValue;
            Group = group;
        }

        // Parameterless constructor with safe defaults
        public NarrativeTone()
        {
            Type = ToneType.Neutral;
            Label = "Tone";
            Category = "Uncategorized";
            BiasValue = Bias.Neutral;
            Group = null;
        }

        // Blend winner against opponent
        public NarrativeTone BlendAgainst(NarrativeTone opponent)
        {
            var label = BiasValue switch
            {
                Bias.Positive => $"{Label} tempered by {opponent.Label}",
                Bias.Negative => $"{Label} deepened by {opponent.Label}",
                Bias.Neutral  => $"{Label} balanced by {opponent.Label}",
                _             => $"{Label} with {opponent.Label}"
            };
            return new NarrativeTone(Type, label, Category, BiasValue, Group);
        }

        // Merge two tones into a neutral equilibrium
        public NarrativeTone MergeNeutral(NarrativeTone other)
        {
            return new NarrativeTone(
                ToneType.Neutral,
                $"Equilibrium of {Label} and {other.Label}",
                Category,
                Bias.Neutral,
                Group ?? other.Group
            );
        }

        public override string ToString() =>
            $"{Type} → {Label} (Category: {Category}, Bias: {BiasValue}, Multiplier: {BiasMultiplier}, Group: {Group})";
    }
}
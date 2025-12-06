using substrate_shared.Registries.enums;

namespace substrate_shared.Registries.Base
{
    public class NarrativeTone
    {
        public string Label { get; }
        public string Category { get; }
        public Bias BiasValue { get; }
        public string? Group { get; }

        // Derived numeric multiplier for math
        public int BiasMultiplier => BiasValue switch
        {
            Bias.Positive => +1,
            Bias.Negative => -1,
            Bias.Neutral  => 0,
            _             => 0
        };

        // Full constructor
        public NarrativeTone(string label, string category, Bias biasValue, string? group = null)
        {
            Label = label;
            Category = category;
            BiasValue = biasValue;
            Group = group;
        }

        // Parameterless constructor with safe defaults
        public NarrativeTone()
        {
            Label = "Tone";
            Category = "Uncategorized";
            BiasValue = Bias.Neutral;
            Group = null;
        }

        // Blend winner against opponent, keeping direction but shading label
        public NarrativeTone BlendAgainst(NarrativeTone opponent)
        {
            var label = BiasValue switch
            {
                Bias.Positive => $"{Label} tempered by {opponent.Label}",
                Bias.Negative => $"{Label} deepened by {opponent.Label}",
                Bias.Neutral  => $"{Label} balanced by {opponent.Label}",
                _             => $"{Label} with {opponent.Label}"
            };
            return new NarrativeTone(label, Category, BiasValue, Group);
        }

        // Merge two tones into a neutral equilibrium
        public NarrativeTone MergeNeutral(NarrativeTone other)
        {
            return new NarrativeTone(
                $"Equilibrium of {Label} and {other.Label}",
                Category,
                Bias.Neutral,
                Group ?? other.Group
            );
        }

        public override string ToString() =>
            $"{Label} (Category: {Category}, Bias: {BiasValue}, Multiplier: {BiasMultiplier}, Group: {Group})";
    }
}
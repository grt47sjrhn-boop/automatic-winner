using System.Text.Json;
using substrate_shared.Registries.enums;

namespace substrate_shared.Registries.Base
{
    public class NarrativeTone
    {
        public ToneType Type { get; }
        public string Label { get; }
        public NarrativeGroup Group { get; }
        public Bias BiasValue { get; }

        public int BiasMultiplier => BiasValue switch
        {
            Bias.Positive => +1,
            Bias.Negative => -1,
            Bias.Neutral  => 0,
            _             => 0
        };

        public NarrativeTone(ToneType type, string label, NarrativeGroup group, Bias bias)
        {
            Type = type;
            Label = label;
            Group = group;
            BiasValue = bias;
        }

        public NarrativeTone() : this(ToneType.Neutral, "Neutral", NarrativeGroup.Equilibrium, Bias.Neutral) { }

        public NarrativeTone BlendAgainst(NarrativeTone opponent)
        {
            var label = BiasValue switch
            {
                Bias.Positive => $"{Label} amplified by {opponent.Label}",
                Bias.Negative => $"{Label} softened by {opponent.Label}",
                Bias.Neutral  => $"{Label} balanced by {opponent.Label}",
                _             => $"{Label} with {opponent.Label}"
            };
            return new NarrativeTone(Type, label, Group, BiasValue);
        }

        public NarrativeTone MergeNeutral(NarrativeTone other)
        {
            return new NarrativeTone(
                ToneType.Neutral,
                $"Equilibrium of {Label} and {other.Label}",
                Group,
                Bias.Neutral
            );
        }

        public string ToJson() =>
            JsonSerializer.Serialize(new
            {
                type = Type.ToString(),
                label = Label,
                group = Group.ToString(),
                bias = BiasValue.ToString(),
                multiplier = BiasMultiplier
            }, new JsonSerializerOptions { WriteIndented = true });

        public override string ToString() =>
            $"{Type} → {Label} (Group: {Group}, Bias: {BiasValue}, Multiplier: {BiasMultiplier})";
    }
}
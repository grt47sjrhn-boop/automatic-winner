using System;
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

        public static NarrativeTone Neutral() =>
            new NarrativeTone(ToneType.Neutral, "Neutral", NarrativeGroup.Equilibrium, Bias.Neutral);

        public NarrativeTone BlendAgainst(NarrativeTone opponent)
        {
            var label = BiasValue switch
            {
                Bias.Positive => $"{Label} amplified by {opponent.Label}",
                Bias.Negative => $"{Label} softened by {opponent.Label}",
                Bias.Neutral  => $"{Label} balanced by {opponent.Label}",
                _             => $"{Label} with {opponent.Label}"
            };

            // Adjust bias if opponent is stronger
            var newBias = opponent.BiasValue == Bias.Neutral ? BiasValue : opponent.BiasValue;

            return new NarrativeTone(Type, label, Group, newBias);
        }

        public NarrativeTone MergeNeutral(NarrativeTone other) =>
            new NarrativeTone(
                ToneType.Neutral,
                $"Equilibrium of {Label} and {other.Label}",
                Group,
                Bias.Neutral
            );

        public string ToJson() =>
            JsonSerializer.Serialize(new
            {
                type = Type.ToString(),
                label = Label,
                group = Group.ToString(),
                bias = BiasValue.ToString(),
                multiplier = BiasMultiplier
            }, new JsonSerializerOptions { WriteIndented = true });

        public static NarrativeTone FromJson(string json)
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var type = Enum.Parse<ToneType>(root.GetProperty("type").GetString()!);
            var label = root.GetProperty("label").GetString()!;
            var group = Enum.Parse<NarrativeGroup>(root.GetProperty("group").GetString()!);
            var bias = Enum.Parse<Bias>(root.GetProperty("bias").GetString()!);

            return new NarrativeTone(type, label, group, bias);
        }

        public override string ToString() =>
            $"{Type} → {Label} (Group: {Group}, Bias: {BiasValue}, Multiplier: {BiasMultiplier})";
    }
}
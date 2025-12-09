using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;

namespace substrate_shared.structs
{
    public class BiasDescriptor
    {
        public Bias Bias { get; set; } = Bias.Neutral;
        public string Narrative { get; set; } = string.Empty;
        public IReadOnlyDictionary<ToneType,int> Facets { get; set; } = new Dictionary<ToneType,int>();

        // 🔹 Numeric scalar value
        public double Value { get; set; } = 0.0;

        // 🔹 Optional registry enrichments
        public MoodType? Mood { get; set; }
        public IntentAction? Intent { get; set; }
        public CrystalRarity? Rarity { get; set; }

        // 🔹 Derived severity band
        public string Severity =>
            Value > 5 ? "Strong Positive"
          : Value < -5 ? "Strong Negative"
          : System.Math.Abs(Value) <= 2 ? "Fragile"
          : "Neutral";

        public BiasDescriptor() { }

        public BiasDescriptor(
            Bias bias,
            string narrative = "",
            IReadOnlyDictionary<ToneType,int>? facets = null,
            double value = 0.0,
            MoodType? mood = null,
            IntentAction? intent = null,
            CrystalRarity? rarity = null)
        {
            Bias = bias;
            Narrative = string.IsNullOrWhiteSpace(narrative)
                ? bias switch
                {
                    Bias.Positive => $"Bias classified as {bias} → resilience and recovery",
                    Bias.Negative => $"Bias classified as {bias} → collapse and despair",
                    Bias.Mixed    => $"Bias classified as {bias} → oscillation and conflict",
                    _             => $"Bias classified as {bias}"
                }
                : narrative;

            Facets = facets ?? new Dictionary<ToneType,int>();
            Value = value;
            Mood = mood;
            Intent = intent;
            Rarity = rarity;
        }

        public override string ToString()
        {
            var facetSummary = Facets != null && Facets.Any()
                ? string.Join(", ", Facets.Select(f => $"{f.Key}({f.Value})"))
                : "none";

            var enrichments = $"{(Mood.HasValue ? $"Mood={Mood}" : "")} " +
                              $"{(Intent.HasValue ? $"Intent={Intent}" : "")} " +
                              $"{(Rarity.HasValue ? $"Rarity={Rarity}" : "")}".Trim();

            return $"{Bias} (Value={Value}, Severity={Severity}): {Narrative} | facets: {facetSummary} | {enrichments}";
        }
    }
}
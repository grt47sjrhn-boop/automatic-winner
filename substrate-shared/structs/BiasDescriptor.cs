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

        public BiasDescriptor() { }

        public BiasDescriptor(Bias bias, string narrative = "", IReadOnlyDictionary<ToneType,int>? facets = null)
        {
            Bias = bias;
            Narrative = string.IsNullOrWhiteSpace(narrative)
                ? $"Bias classified as {bias}"
                : narrative;
            Facets = facets ?? new Dictionary<ToneType,int>();
        }

        public override string ToString()
        {
            var facetSummary = Facets != null && Facets.Any()
                ? string.Join(", ", Facets.Select(f => $"{f.Key}({f.Value})"))
                : "none";
            return $"{Bias}: {Narrative} | facets: {facetSummary}";
        }
    }
}
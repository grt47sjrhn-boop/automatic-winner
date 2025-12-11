using System.Collections.Generic;
using substrate_shared.Traits.Base;

namespace substrate_shared.Traits.Types
{
    public class TraitCrystalGroup
    {
        public string Signature { get; set; } = string.Empty;
        public int Count { get; set; }
        public string DominantTone { get; set; } = string.Empty;
        public string Bias { get; set; } = string.Empty;
        public int MinModifier { get; set; }
        public int MaxModifier { get; set; }

        public List<TraitCrystal> Crystals { get; set; } = new();
        public Dictionary<string, int> MaxFacetValues { get; set; } = new();

        public string DescribeGroup()
        {
            return $"{Signature} ({Count} crystals) — Dominant Tone: {DominantTone}, Bias: {Bias}";
        }
    }
}
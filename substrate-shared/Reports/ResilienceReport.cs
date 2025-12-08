using System.Collections.Generic;
using substrate_shared.Traits.Base;

namespace substrate_shared.Reports
{
    public class ResilienceReport
    {
        // Narrative layer
        public int DuelCount { get; set; }
        public int ResilienceIndex { get; set; }
        public int RecoveryCount { get; set; }
        public int CollapseCount { get; set; }
        public int WoundCount { get; set; }
        public int ConflictCount { get; set; }
        public int EquilibriumCount { get; set; }

        // Math overlay layer
        public double AverageHypotenuse { get; set; }
        public double CumulativeArea { get; set; }
        public double MeanCos { get; set; }
        public double MeanSin { get; set; }
        public double LogScaledIndex { get; set; }
        public double ExpScaledIndex { get; set; }

        // Tone and Intent aggregation
        public Dictionary<string,int> ToneCounts { get; set; } = new();
        public Dictionary<string,int> IntentCounts { get; set; } = new();
        public Dictionary<string,int> ToneLabels { get; set; } = new();

        // 🔹 Crystal inventory (grouped)
        public List<TraitCrystalGroup> CrystalGroups { get; set; } = new();
        public List<TraitCrystal> Crystals { get; set; } = new();
        
        // 🔹 New enriched fields
        public Dictionary<string,int> BrillianceCuts { get; set; } = new();
        public Dictionary<string,int> RarityCounts { get; set; } = new();
        public List<string> CrystalNarratives { get; set; } = new();
        public List<string> BiasSummaries { get; set; } = new();

    }

    public class TraitCrystalGroup
    {
        public string Signature { get; set; } = string.Empty;
        public int Count { get; set; }
        public string DominantTone { get; set; } = string.Empty;
        public string Bias { get; set; } = string.Empty;
        public int MinModifier { get; set; }
        public int MaxModifier { get; set; }

        // Keep strongly typed reference to actual crystals
        public List<TraitCrystal> Crystals { get; set; } = new();
        public Dictionary<string, int> MaxFacetValues { get; set; }
    }
}
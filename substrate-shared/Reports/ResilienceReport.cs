using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Traits.Base;

namespace substrate_shared.Reports
{
    public class ResilienceReport
    {
        // Narrative layer
        public int DuelCount { get; set; }
        public double ResilienceIndex { get; set; }
        public double TotalResilience { get; set; }
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

        // Crystal inventory (grouped)
        public List<TraitCrystalGroup> CrystalGroups { get; set; } = new();
        public List<TraitCrystal> Crystals { get; set; } = new();

        // Enriched fields
        public Dictionary<string,int> BrillianceCuts { get; set; } = new();

        // Collapse rarities
        public Dictionary<string,int> RarityCounts { get; set; } = new()
        {
            { "Common", 0 },
            { "Rare", 0 },
            { "Epic", 0 },
            { "Mythic", 0 },
            { "Legendary", 0 },
            { "UltraRare", 0 },
            { "Fragile", 0 },
            { "Corrupted", 0 },
            { "Doomed", 0 }
        };

        public List<string> CrystalNarratives { get; set; } = new();
        public List<string> BiasSummaries { get; set; } = new();
        public Dictionary<string, int> ToneDistribution { get; set; }
        public Dictionary<string, int> IntentDistribution { get; set; }
        public Dictionary<string, int> CrystalRarity { get; set; }
        public Dictionary<string, int> Outcomes { get; set; }
        public int CrystalCount { get; set; }
        
        public void Print()
        {
            Console.WriteLine("=== Resilience Report ===");
            Console.WriteLine("=== Report Summary ===");
            Console.WriteLine($"Duels: {DuelCount} | Total Resilience: {TotalResilience:F2} | Resilience Index: {ResilienceIndex:F2}");
            Console.WriteLine($"Outcomes → Recoveries: {RecoveryCount}, Collapses: {CollapseCount}, Wounds: {WoundCount}, Conflicts: {ConflictCount}, Equilibriums: {EquilibriumCount}");

            // 🔹 Math overlay values
            Console.WriteLine();
            Console.WriteLine("Math Overlay Metrics:");
            Console.WriteLine($"  Average Hypotenuse: {AverageHypotenuse:F2}");
            Console.WriteLine($"  Cumulative Area:    {CumulativeArea:F2}");
            Console.WriteLine($"  Mean Cos:           {MeanCos:F4}");
            Console.WriteLine($"  Mean Sin:           {MeanSin:F4}");
            Console.WriteLine($"  LogScaled Index:    {LogScaledIndex:F2}");
            Console.WriteLine($"  ExpScaled Index:    {ExpScaledIndex:F2}");

            Console.WriteLine();
            Console.WriteLine("Tone Distribution:");
            foreach (var kvp in ToneLabels)
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");

            Console.WriteLine();
            Console.WriteLine("Intent Distribution:");
            foreach (var kvp in IntentCounts)
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");

            Console.WriteLine();
            Console.WriteLine("Crystal Rarity:");
            foreach (var kvp in RarityCounts)
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");

            Console.WriteLine();
            Console.WriteLine("Crystal Narratives:");
            foreach (var narrative in CrystalNarratives)
                Console.WriteLine($"  - {narrative}");

            Console.WriteLine();
            Console.WriteLine("Bias Summaries:");
            foreach (var bias in BiasSummaries)
                Console.WriteLine($"  - {bias}");
        }
    }

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
    }
}
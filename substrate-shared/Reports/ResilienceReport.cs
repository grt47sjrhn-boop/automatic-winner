using System.Collections.Generic;
using System.Linq;

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

        // New: Tone and Intent aggregation
        public Dictionary<string,int> ToneCounts { get; set; } = new();
        public Dictionary<string,int> IntentCounts { get; set; } = new();

        // New: normalized tone labels
        public Dictionary<string,int> ToneLabels { get; set; } = new();
        
        public override string ToString()
        {
            var tones = ToneCounts.Count > 0 
                ? string.Join(", ", ToneCounts.Select(kv => $"{kv.Key}:{kv.Value}")) 
                : "none";

            var intents = IntentCounts.Count > 0 
                ? string.Join(", ", IntentCounts.Select(kv => $"{kv.Key}:{kv.Value}")) 
                : "none";

            return
                $"Resilience Report → Duels: {DuelCount}, Index: {ResilienceIndex}, " +
                $"Recoveries: {RecoveryCount}, Collapses: {CollapseCount}, Wounds: {WoundCount}, Conflicts: {ConflictCount}, Equilibria: {EquilibriumCount}. " +
                $"Math → AvgHyp: {AverageHypotenuse:F2}, CumArea: {CumulativeArea:F2}, MeanCos: {MeanCos:F2}, MeanSin: {MeanSin:F2}, " +
                $"LogIndex: {LogScaledIndex:F2}, ExpIndex: {ExpScaledIndex:F2}. " +
                $"Tones → {tones}. Intents → {intents}";
        }
    }
}
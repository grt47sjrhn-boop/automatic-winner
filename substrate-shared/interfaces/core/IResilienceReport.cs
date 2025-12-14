using System.Collections.Generic;

namespace substrate_shared.interfaces.core
{
    public interface IResilienceReport
    {
        // --- Existing properties ---
        int DuelCount { get; }
        double ResilienceIndex { get; }
        double TotalResilience { get; }
        int RecoveryCount { get; }
        int CollapseCount { get; }
        int WoundCount { get; }
        int ConflictCount { get; }
        int EquilibriumCount { get; }

        double AverageHypotenuse { get; }
        double CumulativeArea { get; }
        double MeanCos { get; }
        double MeanSin { get; }
        double LogScaledIndex { get; }
        double ExpScaledIndex { get; }

        IReadOnlyDictionary<string,int> ToneDistribution { get; }
        IReadOnlyDictionary<string,int> IntentDistribution { get; }
        IReadOnlyDictionary<string,int> RarityCounts { get; }

        IReadOnlyList<string> CrystalNarratives { get; }
        IReadOnlyList<string> BiasSummaries { get; }
        int CrystalCount { get; }
        IReadOnlyDictionary<string,int> Outcomes { get; }
        IReadOnlyDictionary<string,int> CrystalRarity { get; }
        IReadOnlyDictionary<string,int> BrillianceCuts { get; }

        string GenerateNarrative();

        // --- Codex Extensions ---

        // Catalyst: meta-state overlays
        IReadOnlyDictionary<string,double> MetaStateWeights { get; }
        IReadOnlyList<string> MetaStateNarratives { get; }
        
        // Builder methods
        void AddMetaStateWeight(string key, double value);
        void AddMetaStateNarrative(string narrative);
        
        // Crucible: Intent cluster (renamed from WheelCluster)
        IReadOnlyList<string> IntentCluster { get; }
        IReadOnlyDictionary<string,double> ClusterWeights { get; }

        // Oracle: arcs, epochs, rarity modulation
        IReadOnlyList<string> Epochs { get; }
        IReadOnlyList<string> ArcTriggers { get; }
        IReadOnlyDictionary<string,double> RarityModulation { get; }

        // Codex entry generator
        string GenerateCodexEntry();
    }
}
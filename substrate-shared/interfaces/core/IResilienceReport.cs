using System.Collections.Generic;

namespace substrate_shared.interfaces.core
{
    public interface IResilienceReport
    {
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
    }
}
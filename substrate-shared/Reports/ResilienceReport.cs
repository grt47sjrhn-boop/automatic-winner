using System;
using System.Collections.Generic;
using substrate_shared.interfaces.core;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Types;

namespace substrate_shared.Reports
{
    public class ResilienceReport : IResilienceReport
    {
        // Properties (readonly to consumers)
        public int DuelCount { get; private set; }
        public double ResilienceIndex { get; private set; }
        public double TotalResilience { get; private set; }
        public int RecoveryCount { get; private set; }
        public int CollapseCount { get; private set; }
        public int WoundCount { get; private set; }
        public int ConflictCount { get; private set; }
        public int EquilibriumCount { get; private set; }

        public double AverageHypotenuse { get; private set; }
        public double CumulativeArea { get; private set; }
        public double MeanCos { get; private set; }
        public double MeanSin { get; private set; }
        public double LogScaledIndex { get; private set; }
        public double ExpScaledIndex { get; private set; }

        public IReadOnlyDictionary<string,int> ToneDistribution { get; private set; } = new Dictionary<string,int>();
        public IReadOnlyDictionary<string,int> IntentDistribution { get; private set; } = new Dictionary<string,int>();
        public IReadOnlyDictionary<string,int> RarityCounts { get; private set; } = new Dictionary<string,int>();

        public IReadOnlyList<string> CrystalNarratives { get; private set; } = new List<string>();
        public IReadOnlyList<string> BiasSummaries { get; private set; } = new List<string>();
        public int CrystalCount { get; private set; }
        public IReadOnlyDictionary<string,int> Outcomes { get; private set; } = new Dictionary<string,int>();
        public IReadOnlyDictionary<string,int> CrystalRarity { get; private set; } = new Dictionary<string,int>();
        public IReadOnlyDictionary<string,int> BrillianceCuts { get; private set; } = new Dictionary<string,int>();

        /// <summary>
        /// Public method to populate all readonly properties.
        /// </summary>
        public void SetMetrics(
            int duelCount, double resilienceIndex, double totalResilience,
            int recoveryCount, int collapseCount, int woundCount, int conflictCount, int equilibriumCount,
            double avgHypotenuse, double cumulativeArea, double meanCos, double meanSin,
            double logScaledIndex, double expScaledIndex,
            Dictionary<string,int> toneDistribution,
            Dictionary<string,int> intentDistribution,
            List<TraitCrystalGroup> crystalGroups,
            List<TraitCrystal> crystals,
            Dictionary<string,int> rarityCounts,
            List<string> crystalNarratives,
            List<string> biasSummaries,
            int crystalCount,
            Dictionary<string,int> outcomes,
            Dictionary<string,int> crystalRarity,
            Dictionary<string,int> brillianceCuts)
        {
            DuelCount = duelCount;
            ResilienceIndex = resilienceIndex;
            TotalResilience = totalResilience;
            RecoveryCount = recoveryCount;
            CollapseCount = collapseCount;
            WoundCount = woundCount;
            ConflictCount = conflictCount;
            EquilibriumCount = equilibriumCount;

            AverageHypotenuse = avgHypotenuse;
            CumulativeArea = cumulativeArea;
            MeanCos = meanCos;
            MeanSin = meanSin;
            LogScaledIndex = logScaledIndex;
            ExpScaledIndex = expScaledIndex;

            ToneDistribution = toneDistribution;
            IntentDistribution = intentDistribution;
            RarityCounts = rarityCounts;
            CrystalNarratives = crystalNarratives;
            BiasSummaries = biasSummaries;
            CrystalCount = crystalCount;
            Outcomes = outcomes;
            CrystalRarity = crystalRarity;
            BrillianceCuts = brillianceCuts;
        }

        public string GenerateNarrative()
        {
            if (AverageHypotenuse > 10 && CumulativeArea > 50)
                return "Battles sprawled wide and expansive, forging rarer salvage.";
            if (AverageHypotenuse < 5 && CumulativeArea < 20)
                return "Duels collapsed inward, yielding only common scraps.";
            return "Resilience oscillated across balanced duels, producing mixed salvage.";
        }
    }
}
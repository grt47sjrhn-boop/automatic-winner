using System;
using System.Collections.Generic;
using substrate_shared.interfaces.core;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Types;

namespace substrate_shared.Reports
{
    public class ResilienceReport : IResilienceReport
    {
        // --- Existing properties ---
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

        // --- Codex Extensions ---
        public IReadOnlyDictionary<string,double> MetaStateWeights { get; private set; } = new Dictionary<string,double>();
        public IReadOnlyList<string> MetaStateNarratives { get; private set; } = new List<string>();

        public IReadOnlyList<string> IntentCluster { get; private set; } = new List<string>();
        public IReadOnlyDictionary<string,double> ClusterWeights { get; private set; } = new Dictionary<string,double>();

        public IReadOnlyList<string> Epochs { get; private set; } = new List<string>();
        public IReadOnlyList<string> ArcTriggers { get; private set; } = new List<string>();
        public IReadOnlyDictionary<string,double> RarityModulation { get; private set; } = new Dictionary<string,double>();

        // --- Existing methods ---
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

        // --- Codex setters ---
        public void SetMetaStates(Dictionary<string,double> weights, List<string> narratives)
        {
            MetaStateWeights = weights;
            MetaStateNarratives = narratives;
        }

        public void SetIntentCluster(List<string> cluster, Dictionary<string,double> weights)
        {
            IntentCluster = cluster;
            ClusterWeights = weights;
        }

        public void SetEpochs(List<string> epochs, List<string> triggers, Dictionary<string,double> rarityModulation)
        {
            Epochs = epochs;
            ArcTriggers = triggers;
            RarityModulation = rarityModulation;
        }

        // --- Narrative methods ---
        public string GenerateNarrative()
        {
            if (AverageHypotenuse > 10 && CumulativeArea > 50)
                return "Battles sprawled wide and expansive, forging rarer salvage.";
            if (AverageHypotenuse < 5 && CumulativeArea < 20)
                return "Duels collapsed inward, yielding only common scraps.";
            return "Resilience oscillated across balanced duels, producing mixed salvage.";
        }

        public string GenerateCodexEntry()
        {
            var baseNarrative = GenerateNarrative();
            var metaOverlay = MetaStateNarratives.Count > 0 ? string.Join(" ", MetaStateNarratives) : "";
            var epochSummary = Epochs.Count > 0 ? $"Epochs unfolding: {string.Join(", ", Epochs)}." : "";
            return $"{baseNarrative} {metaOverlay} {epochSummary}".Trim();
        }
    }
}
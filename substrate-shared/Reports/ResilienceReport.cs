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

        // --- Codex Extensions (backed by mutable collections) ---
        private readonly Dictionary<string,double> _metaStateWeights = new();
        private readonly List<string> _metaStateNarratives = [];
        public IReadOnlyDictionary<string,double> MetaStateWeights => _metaStateWeights;
        public IReadOnlyList<string> MetaStateNarratives => _metaStateNarratives;

        public void AddMetaStateWeight(string key, double value) => _metaStateWeights[key] = value;
        public void AddMetaStateNarrative(string narrative) => _metaStateNarratives.Add(narrative);

        private readonly List<string> _intentCluster = [];
        private readonly Dictionary<string,double> _clusterWeights = new();
        public IReadOnlyList<string> IntentCluster => _intentCluster;
        public IReadOnlyDictionary<string,double> ClusterWeights => _clusterWeights;

        private readonly List<string> _epochs = [];
        private readonly List<string> _arcTriggers = [];
        private readonly Dictionary<string,double> _rarityModulation = new();
        public IReadOnlyList<string> Epochs => _epochs;
        public IReadOnlyList<string> ArcTriggers => _arcTriggers;
        public IReadOnlyDictionary<string,double> RarityModulation => _rarityModulation;

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
            _metaStateWeights.Clear();
            foreach (var kvp in weights)
                _metaStateWeights[kvp.Key] = kvp.Value;

            _metaStateNarratives.Clear();
            _metaStateNarratives.AddRange(narratives);
        }

        public void SetIntentCluster(List<string> cluster, Dictionary<string,double> weights)
        {
            _intentCluster.Clear();
            _intentCluster.AddRange(cluster);

            _clusterWeights.Clear();
            foreach (var kvp in weights)
                _clusterWeights[kvp.Key] = kvp.Value;
        }

        public void SetEpochs(List<string> epochs, List<string> triggers, Dictionary<string,double> rarityModulation)
        {
            _epochs.Clear();
            _epochs.AddRange(epochs);

            _arcTriggers.Clear();
            _arcTriggers.AddRange(triggers);

            _rarityModulation.Clear();
            foreach (var kvp in rarityModulation)
                _rarityModulation[kvp.Key] = kvp.Value;
        }

        // --- Narrative methods ---
        public string GenerateNarrative()
        {
            return AverageHypotenuse switch
            {
                > 10 when CumulativeArea > 50 => "Battles sprawled wide and expansive, forging rarer salvage.",
                < 5 when CumulativeArea < 20 => "Duels collapsed inward, yielding only common scraps.",
                _ => "Resilience oscillated across balanced duels, producing mixed salvage."
            };
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
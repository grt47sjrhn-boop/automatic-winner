using System.Collections.Generic;
using substrate_shared.Reports;
using substrate_shared.structs;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Types;

namespace substrate_shared.interfaces.core
{
    /// <summary>
    /// Contract for tracking duel outcomes, overlays, and crystals,
    /// then producing resilience reports.
    /// </summary>
    public interface IResilienceTracker
    {
        public IReadOnlyList<ISummary> DuelSummaries { get; }
        IReadOnlyList<TraitCrystal> Crystals { get; }
        IReadOnlyList<string> Narratives { get; }

        double TotalResilience { get; }
        double ResilienceIndex { get; }
        double AverageHypotenuse { get; }
        double CumulativeArea { get; }
        double MeanCos { get; }
        double MeanSin { get; }
        double LogScaledIndex { get; }
        double ExpScaledIndex { get; }

        ISummary Record(ISummary summary, BiasVector? a = null, BiasVector? b = null);

        /// <summary>
        /// Build a resilience report DTO with aggregate metrics, distributions, and crystal data.
        /// </summary>
        IResilienceReport ComputeResilience();

        void AddHypotenuse(double value);
        void AddArea(double value);
        void AddTrig(double cos, double sin, double log, double exp);

        void AddSummary(ISummary summary);
        void AddCrystal(TraitCrystal crystal);
        void AddResilience(double engagementCumulativeResilience);
        void AddNarrative(string description);

        List<ISummary> GetSummaries();
        List<TraitCrystal> GetCrystals();
        IReadOnlyList<TraitCrystalGroup> CrystalGroups { get; }

    }
}
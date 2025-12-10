using System.Collections.Generic;
using substrate_shared.Reports;
using substrate_shared.structs;
using substrate_shared.Traits.Base;

namespace substrate_shared.interfaces
{
    public interface IResilienceTracker
    {
        // Narrative-facing properties
        IReadOnlyList<ISummary> DuelSummaries { get; }
        // ✅ Expose both cumulative and averaged resilience
        double TotalResilience { get; }
        double ResilienceIndex { get; }


        // Crystal inventory
        IReadOnlyList<TraitCrystal> Crystals { get; }

        // Existing methods
        ISummary Record(ISummary summary, BiasVector? a = null, BiasVector? b = null);
        ResilienceReport ComputeResilience();

        // 🔹 Math overlay properties
        double AverageHypotenuse { get; }
        double CumulativeArea { get; }
        double MeanCos { get; }
        double MeanSin { get; }
        double LogScaledIndex { get; }
        double ExpScaledIndex { get; }

        void AddHypotenuse(double value);
        void AddArea(double value);
        void AddTrig(double cos, double sin, double log, double exp);

        // 🔹 New methods
        void AddSummary(ISummary summary);
        void AddCrystal(TraitCrystal crystal);

        List<ISummary> GetSummaries();
        List<TraitCrystal> GetCrystals();

        // 🔹 Explicit numeric type for resilience
        void AddResilience(double engagementCumulativeResilience);
        void AddNarrative(string description);
    }
}
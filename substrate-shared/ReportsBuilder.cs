using System.Linq;
using substrate_core.Reporting;
using substrate_shared.interfaces;

namespace substrate_shared
{
    public class ReportBuilder
    {
        private readonly IResilienceTracker _tracker;

        public ReportBuilder(IResilienceTracker tracker)
        {
            _tracker = tracker;
        }

        public ResilienceReport BuildReport()
        {
            return new ResilienceReport
            {
                DuelCount        = _tracker.DuelSummaries.Count,
                ResilienceIndex  = _tracker.ResilienceIndex,

                // Narrative counts
                RecoveryCount    = _tracker.DuelSummaries.Count(s => s.Description.Contains("Recovery")),
                CollapseCount    = _tracker.DuelSummaries.Count(s => s.Description.Contains("Collapse")),
                WoundCount       = _tracker.DuelSummaries.Count(s => s.Description.Contains("Wound")),
                ConflictCount    = _tracker.DuelSummaries.Count(s => s.Description.Contains("MixedConflict")),
                EquilibriumCount = _tracker.DuelSummaries.Count(s => s.Description.Contains("Equilibrium")),

                // Math overlay values
                AverageHypotenuse = _tracker.AverageHypotenuse,
                CumulativeArea    = _tracker.CumulativeArea,
                MeanCos           = _tracker.MeanCos,
                MeanSin           = _tracker.MeanSin,
                LogScaledIndex    = _tracker.LogScaledIndex,
                ExpScaledIndex    = _tracker.ExpScaledIndex
            };
        }
    }
}
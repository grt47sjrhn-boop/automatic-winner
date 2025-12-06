using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.types;
using substrate_shared.Overlays;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;

namespace substrate_core.Resolvers
{
    public class ResilienceTracker : IResilienceTracker
    {
        private readonly List<ISummary> _duelSummaries = new();
        private readonly List<(BiasVector, BiasVector)> _duelPairs = new();
        private int _resilienceIndex = 0;

        public IReadOnlyList<ISummary> DuelSummaries => _duelSummaries;
        public int ResilienceIndex => _resilienceIndex;

        // 🔹 Math overlay properties exposed via interface
        public double AverageHypotenuse => TrigOverlay.AverageHypotenuse(_duelPairs);
        public double CumulativeArea => TrigOverlay.CumulativeArea(_duelPairs);
        public double MeanCos => TrigOverlay.MeanCos(_duelPairs);
        public double MeanSin => TrigOverlay.MeanSin(_duelPairs);
        public double LogScaledIndex => TrigOverlay.LogScaledIndex(_resilienceIndex);
        public double ExpScaledIndex => TrigOverlay.ExpScaledIndex(_resilienceIndex);

        // Record duel summary and optional vector pair for math overlay
        public ISummary Record(ISummary summary, BiasVector? a = null, BiasVector? b = null)
        {
            _duelSummaries.Add(summary);

            if (summary is DuelEventSummary duelSummary)
            {
                switch (duelSummary.Outcome)
                {
                    case DuelOutcome.Recovery: _resilienceIndex += 2; break;
                    case DuelOutcome.Collapse: _resilienceIndex -= 2; break;
                    case DuelOutcome.Wound:    _resilienceIndex -= 1; break;
                    case DuelOutcome.Conflict: _resilienceIndex -= 1; break;
                    case DuelOutcome.Equilibrium: _resilienceIndex += 1; break;
                }
            }

            if (a.HasValue && b.HasValue)
                _duelPairs.Add((a.Value, b.Value));

            return BuildOverlay();
        }


        private ISummary BuildOverlay()
        {
            int recoveries = _duelSummaries.Count(s => s.Description.Contains("Recovery"));
            int collapses  = _duelSummaries.Count(s => s.Description.Contains("Collapse"));
            int wounds     = _duelSummaries.Count(s => s.Description.Contains("Wound"));
            int conflicts  = _duelSummaries.Count(s => s.Description.Contains("MixedConflict"));

            string description =
                $"Trajectory: {recoveries} recoveries, {collapses} collapses, {wounds} wounds, {conflicts} conflicts. " +
                $"Resilience Index: {_resilienceIndex}. " +
                $"Math Overlay → AvgHyp: {AverageHypotenuse:F2}, CumArea: {CumulativeArea:F2}, " +
                $"MeanCos: {MeanCos:F2}, MeanSin: {MeanSin:F2}, " +
                $"LogIndex: {LogScaledIndex:F2}, ExpIndex: {ExpScaledIndex:F2}";

            return new EventSummary(
                $"Resilience Overlay (after {_duelSummaries.Count} duel{(_duelSummaries.Count == 1 ? "" : "s")})",
                description,
                SummaryType.System,
                false
            );
        }

        public ISummary ComputeResilience()
        {
            if (!_duelSummaries.Any())
                return new EventSummary("Resilience Report", "No duels recorded yet.", SummaryType.System, false);

            string description;
            bool resolved = false;

            if (_resilienceIndex >= 6)
            {
                description = $"System crystallizes with resilience index {_resilienceIndex}.";
                resolved = true;
            }
            else if (_resilienceIndex <= -6)
            {
                description = $"System collapses into fragments with resilience index {_resilienceIndex}.";
                resolved = true;
            }
            else
            {
                description = $"System remains oscillating with resilience index {_resilienceIndex}.";
            }

            return new EventSummary("Resilience Report", description, SummaryType.System, resolved);
        }
    }
}
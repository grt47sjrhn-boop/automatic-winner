using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Profiles;
using substrate_shared.Registries.enums;
using substrate_shared.Services.Codex;

namespace substrate_core.Internal.Engines
{
    /// <summary>
    /// Catalyst Engine applies grouping modifiers (Panic, Dread, Resolve, Composure)
    /// to the active Codex context (Report, Summary, Duelist, Inventory).
    /// </summary>
    public sealed class CatalystEngine
    {
        private bool _enabled;

        public CatalystEngine(bool enabled = true)
        {
            _enabled = enabled;
        }

        public void Apply()
        {
            if (!_enabled) return;

            var context = CodexContextService.Instance;

            var report = context.Report;
            var summary = context.Summary;
            var duelist = context.Duelist;
            var inventory = context.Inventory;

            if (report == null || summary == null || duelist == null || inventory == null)
            {
                // Context not fully populated, skip safely
                return;
            }

            // Deterministic modifier application
            ApplyModifiers(report, summary, duelist);
        }

        private void ApplyModifiers(IResilienceReport report, ISummary summary, IDuelist duelist)
        {
            var duels = duelist.GetAllDuels().ToList();
            if (duels.Count == 0) return;

            var bias = summary.Bias;

            // Moving average over last 5 duels
            double woundTrend = MovingAverageWounds(duels, 5);

            // Streak detection
            int woundStreak = LongestStreak(duels, DuelOutcome.Wound);
            int recoveryStreak = LongestStreak(duels, DuelOutcome.Recovery);

            // Narrative overlays
            if (woundTrend > 0.6 || woundStreak >= 3)
            {
                if (bias.Value < 0)
                {
                    report.AddMetaStateWeight("Dread", woundTrend);
                    report.AddMetaStateNarrative(
                        $"Recent wounds trending high ({woundTrend:P0}) with streak {woundStreak} under {bias.Severity} negative bias — dread rising."
                    );
                }
                else if (bias.Value > 0)
                {
                    report.AddMetaStateWeight("Composure", woundTrend);
                    report.AddMetaStateNarrative(
                        $"Recent wounds trending high ({woundTrend:P0}) but recoveries streak {recoveryStreak} under {bias.Severity} positive bias — composure stabilizing."
                    );
                }
                else
                {
                    report.AddMetaStateWeight("Equilibrium", woundTrend);
                    report.AddMetaStateNarrative(
                        $"Recent wounds trending high ({woundTrend:P0}) under neutral bias — state remains balanced."
                    );
                }
            }
        }

        public void Toggle(bool enabled) => _enabled = enabled;

        private int LongestStreak(IEnumerable<ISummary> duels, DuelOutcome target)
        {
            int current = 0, longest = 0;
            foreach (var duel in duels)
            {
                if (duel.Outcome == target)
                {
                    current++;
                    longest = Math.Max(longest, current);
                }
                else current = 0;
            }
            return longest;
        }

        private double MovingAverageWounds(IEnumerable<ISummary> duels, int windowSize)
        {
            var recent = duels.TakeLast(windowSize).ToList();
            if (recent.Count == 0) return 0.0;
            int wounds = recent.Count(d => d.Outcome == DuelOutcome.Wound);
            return (double)wounds / recent.Count;
        }
    }
}
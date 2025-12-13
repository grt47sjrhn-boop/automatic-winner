using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.DTO.Codex;
using substrate_shared.Enums;
using substrate_shared.interfaces.Codex;
using substrate_shared.interfaces.core;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.Services.Codex
{
    public sealed class EpochSchedulerService : IService
    {
        private int _chapterIndex = 0;

        public event Action<CodexEpoch> OnEpochStarted;
        public event Action<CodexEpoch> OnEpochCompleted;

        public void Initialize() { }

        public void Reset()
        {
            _chapterIndex = 0;
        }

        public void Dispose()
        {
            _chapterIndex = 0;
        }

        public void Shutdown() { }

        public CodexEpoch BeginEpoch(IEnumerable<IDuelEventSummary> duels, int windowSize, WindowType type, int currentTick)
        {
            _chapterIndex++;

            var windowDuels = type == WindowType.Count
                ? duels.Take(windowSize)
                : duels.Where(d => d.Tick >= currentTick - windowSize);

            var descriptors = windowDuels.Select(d => d.Bias).ToList();

            // 🔹 Aggregate bias counts
            int positive = descriptors.Count(d => d.Bias == Bias.Positive);
            int negative = descriptors.Count(d => d.Bias == Bias.Negative);
            int neutral  = descriptors.Count(d => d.Bias == Bias.Neutral);
            int mixed    = descriptors.Count(d => d.Bias == Bias.Mixed);

            // 🔹 Average scalar value
            double avgValue = descriptors.Any() ? descriptors.Average(d => d.Value) : 0.0;

            // 🔹 Dominant severity
            string dominantSeverity = descriptors
                .GroupBy(d => d.Severity)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "None";

            // 🔹 Aggregate facets
            var facetTotals = new Dictionary<ToneType, int>();
            foreach (var desc in descriptors)
            {
                foreach (var facet in desc.Facets)
                {
                    if (!facetTotals.ContainsKey(facet.Key))
                        facetTotals[facet.Key] = 0;
                    facetTotals[facet.Key] += facet.Value;
                }
            }

            var epoch = new CodexEpoch(
                _chapterIndex,
                currentTick,
                DateTime.UtcNow,
                windowDuels.Select(d => d.Description).ToList().AsReadOnly(),
                $"Chapter {_chapterIndex}: {dominantSeverity} cadence",
                positive,
                negative,
                neutral,
                mixed,
                avgValue,
                dominantSeverity,
                facetTotals
            );

            OnEpochStarted?.Invoke(epoch);
            return epoch;
        }

        public CodexEpoch CompleteEpoch(CodexEpoch epoch, string resolvedBias, string heading)
        {
            var completed = new CodexEpoch(
                epoch.ChapterIndex,
                epoch.TickMarker,
                epoch.StartedAt,
                epoch.FlavorKeys,
                heading,
                epoch.PositiveCount,
                epoch.NegativeCount,
                epoch.NeutralCount,
                epoch.MixedCount,
                epoch.AverageValue,
                resolvedBias, // override dominant severity with resolved bias string
                epoch.FacetTotals
            );

            OnEpochCompleted?.Invoke(completed);
            return completed;
        }
    }
}
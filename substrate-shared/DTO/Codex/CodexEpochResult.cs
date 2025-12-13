using System;
using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.DTO.Codex
{
    /// <summary>
    /// Structured result of an epoch, aggregating duel-level bias descriptors.
    /// </summary>
    public sealed class CodexEpochResult
    {
        public int ChapterIndex { get; }
        public int TickMarker { get; }
        public DateTime StartedAt { get; }

        // 🔹 Bias aggregates
        public int PositiveCount { get; }
        public int NegativeCount { get; }
        public int NeutralCount { get; }
        public int MixedCount { get; }

        public double AverageValue { get; }
        public string DominantSeverity { get; }

        // 🔹 Facet distribution across epoch
        public IReadOnlyDictionary<ToneType, int> FacetTotals { get; }

        // 🔹 Narrative heading
        public string Heading { get; }

        public CodexEpochResult(
            int chapterIndex,
            int tickMarker,
            DateTime startedAt,
            int positiveCount,
            int negativeCount,
            int neutralCount,
            int mixedCount,
            double averageValue,
            string dominantSeverity,
            IReadOnlyDictionary<ToneType, int> facetTotals,
            string heading)
        {
            ChapterIndex = chapterIndex;
            TickMarker = tickMarker;
            StartedAt = startedAt;
            PositiveCount = positiveCount;
            NegativeCount = negativeCount;
            NeutralCount = neutralCount;
            MixedCount = mixedCount;
            AverageValue = averageValue;
            DominantSeverity = dominantSeverity;
            FacetTotals = facetTotals;
            Heading = heading;
        }
    }
}
using System;
using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.DTO.Codex
{
    /// <summary>
    /// Unified epoch DTO: combines narrative snapshot and structured aggregates.
    /// </summary>
    public sealed class CodexEpoch
    {
        public int ChapterIndex { get; }
        public int TickMarker { get; }
        public DateTime StartedAt { get; }

        // 🔹 Narrative fields
        public IReadOnlyList<string> FlavorKeys { get; }
        public string NarrativeHeading { get; }

        // 🔹 Bias aggregates
        public int PositiveCount { get; }
        public int NegativeCount { get; }
        public int NeutralCount { get; }
        public int MixedCount { get; }

        public double AverageValue { get; }
        public string DominantSeverity { get; }

        // 🔹 Facet distribution
        public IReadOnlyDictionary<ToneType, int> FacetTotals { get; }

        public CodexEpoch(
            int chapterIndex,
            int tickMarker,
            DateTime startedAt,
            IReadOnlyList<string> flavorKeys,
            string narrativeHeading,
            int positiveCount,
            int negativeCount,
            int neutralCount,
            int mixedCount,
            double averageValue,
            string dominantSeverity,
            IReadOnlyDictionary<ToneType, int> facetTotals)
        {
            ChapterIndex = chapterIndex;
            TickMarker = tickMarker;
            StartedAt = startedAt;
            FlavorKeys = flavorKeys;
            NarrativeHeading = narrativeHeading;
            PositiveCount = positiveCount;
            NegativeCount = negativeCount;
            NeutralCount = neutralCount;
            MixedCount = mixedCount;
            AverageValue = averageValue;
            DominantSeverity = dominantSeverity;
            FacetTotals = facetTotals;
        }
    }
}
using System;
using System.Collections.Generic;

namespace substrate_shared.DTO.Codex
{
    public sealed class CodexEntry
    {
        public string Id { get; }
        public string Chapter { get; }
        public string Heading { get; }
        public string Text { get; }
        public IReadOnlyDictionary<string, object> Data { get; }

        /// <summary>
        /// Construct a CodexEntry from raw values.
        /// </summary>
        public CodexEntry(string id, string chapter, string heading, string text, IReadOnlyDictionary<string, object> data)
        {
            Id = id;
            Chapter = chapter;
            Heading = heading;
            Text = text;
            Data = data;
        }

        /// <summary>
        /// Construct a CodexEntry directly from a CodexEpoch.
        /// </summary>
        public CodexEntry(string id, CodexEpoch epoch)
        {
            Id = id;
            Chapter = $"Chapter {epoch.ChapterIndex}";
            Heading = epoch.NarrativeHeading;
            Text = string.Join(Environment.NewLine, epoch.FlavorKeys);

            // 🔹 Capture structured epoch details into Data dictionary
            Data = new Dictionary<string, object>
            {
                { "ChapterIndex", epoch.ChapterIndex },
                { "TickMarker", epoch.TickMarker },
                { "StartedAt", epoch.StartedAt },
                { "PositiveCount", epoch.PositiveCount },
                { "NegativeCount", epoch.NegativeCount },
                { "NeutralCount", epoch.NeutralCount },
                { "MixedCount", epoch.MixedCount },
                { "AverageValue", epoch.AverageValue },
                { "DominantSeverity", epoch.DominantSeverity },
                { "FacetTotals", epoch.FacetTotals }
            };
        }
    }
}
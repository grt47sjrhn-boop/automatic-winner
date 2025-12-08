using System;
using substrate_shared.types;

namespace substrate_shared.Summaries.Base
{
    public class EventSummary(string title, string description, SummaryType type, bool isResolved = false)
        : SummaryBase
    {
        public override string Title { get; } = title ?? throw new ArgumentNullException(nameof(title));
        public override string Description { get; } = description ?? throw new ArgumentNullException(nameof(description));
        public SummaryType Type { get; } = type;
        public bool IsResolved { get; } = isResolved; // only meaningful for Duel

        public override void Print()
        {
            var resolvedText = Type == SummaryType.Duel ? $" (Resolved: {IsResolved})" : string.Empty;
            Console.WriteLine($"{Title} - {Description} [{Type}]{resolvedText}");
        }
    }
}
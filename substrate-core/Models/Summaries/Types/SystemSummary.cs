using System;
using substrate_core.Models.Summaries.Base;

namespace substrate_core.Models.Summaries.Types
{
    public class SystemSummary : SummaryBase
    {
        public override string Title { get; }
        public override string Description { get; }

        public SystemSummary(string title, string description, string category)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
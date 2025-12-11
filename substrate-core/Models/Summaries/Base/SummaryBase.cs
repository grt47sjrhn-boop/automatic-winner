using System;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.types;

namespace substrate_core.Models.Summaries.Base
{
    public abstract class SummaryBase : ISummary
    {
        public abstract string Title { get; }
        public abstract string Description { get; }

        public virtual void Print() => Console.WriteLine($"{Title} - {Description}");
        public override string ToString() => $"{Title} - {Description}";

        // --- Core fields ---
        public virtual BiasVector ResolvedVector { get; protected set; }
        public virtual FacetDistribution Shape { get; protected set; } = new();
        public bool IsResolved { get; }
        public double ResilienceIndex { get; set; }
        public BiasDescriptor Bias { get; }

        public virtual DuelOutcome Outcome { get; protected set; }
        public virtual SummaryType SummaryType { get; protected set; } = SummaryType.System;
        public virtual IToneCut Brilliance { get; protected set; }
        public virtual IntentAction Intent { get; set; } = IntentAction.Observe;
    }
}
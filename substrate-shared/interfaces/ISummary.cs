using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.types;

namespace substrate_shared.interfaces
{
    public interface ISummary
    {
        string Title { get; }
        string Description { get; }
        void Print();
        string ToString();

        // New structured fields
        BiasVector ResolvedVector { get; }
        DuelOutcome Outcome { get; }
        /// <summary>
        /// Higher-level summary classification (Positive, Negative, Mixed, Neutral).
        /// This is what EventSummary expects in its constructor.
        /// </summary>
        SummaryType SummaryType { get; }

        ToneCut Brilliance { get; }
        IntentAction Intent { get; set; }
        FacetDistribution Shape { get; }
        bool IsResolved { get; }
        double ResilienceIndex { get; }
        BiasDescriptor Bias { get; }
    }
}
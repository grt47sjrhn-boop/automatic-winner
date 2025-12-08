using substrate_shared.Registries.enums;
using substrate_shared.types;

namespace substrate_shared.Mappers
{
    public static class SummaryTypeMapper
    {
        /// <summary>
        /// Map a DuelOutcome into a SummaryType for EventSummary.
        /// </summary>
        public static SummaryType FromDuelOutcome(DuelOutcome outcome)
        {
            return outcome switch
            {
                DuelOutcome.Recovery   => SummaryType.Positive,
                DuelOutcome.Collapse   => SummaryType.Negative,
                DuelOutcome.Stalemate  => SummaryType.Neutral,
                DuelOutcome.Unresolved => SummaryType.Mixed,
                _                      => SummaryType.Mixed
            };
        }
    }
}
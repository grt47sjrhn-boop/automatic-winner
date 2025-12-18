namespace substrate_shared.Types.Enums
{
    public static class PrismTypes
    {
        // =======================
        // Bias
        // =======================
        public enum BiasTag
        {
            Fear,
            Greed,
            Loyalty,
            Distrust
        }

        // =======================
        // Closure
        // =======================
        public enum ClosureState
        {
            UnifiedClosure,   // Crew succeeded together
            Conflict,         // Crew argued or stalled
            Escalation,       // Situation worsened
            Resolution        // Partial or negotiated closure
        }

        // =======================
        // Intent
        // =======================
        public enum IntentType
        {
            Dig,
            Retreat,
            NegotiateReturn
        }

        // =======================
        // Source System
        // =======================
        public enum SourceSystem
        {
            MiningOps
        }

        // =======================
        // Narration Tier
        // =======================
        public enum NarrationTier
        {
            Minimal,
            Detailed,
            Mythic
        }

        // =======================
        // Trigger IDs
        // =======================
        public enum TriggerID
        {
            CollapseEvent,
            RichVeinFound,
            RumorSpread
        }
    }
}
using System;
using substrate_shared.Models.Types;
using substrate_shared.Types;

namespace substrate_core.Orchestration
{
    public class ConstellationOrchestrator
    {
        private Random rng = new Random();

        public ClosureOutput ProcessIntent(
            InputIntent intent,
            PrismTypes.BiasTag dominantBias,
            string trigger,
            string narrationTier)
        {
            var closure = new ClosureOutput();

            // Simplified closure logic
            if (dominantBias == PrismTypes.BiasTag.Fear && rng.NextDouble() > 0.4)
            {
                closure.ClosureState = PrismTypes.ClosureState.Escalation;
                closure.NarrationText = $"{intent.ActorID} panicked after {trigger}.";
            }
            else if (dominantBias == PrismTypes.BiasTag.Greed && rng.NextDouble() > 0.5)
            {
                closure.ClosureState = PrismTypes.ClosureState.UnifiedClosure;
                closure.NarrationText = $"{intent.ActorID} pressed deeper, driven by greed.";
            }
            else
            {
                closure.ClosureState = PrismTypes.ClosureState.Conflict;
                closure.NarrationText = $"{intent.ActorID} argued among themselves, indecisive.";
            }

            return closure;
        }
    }

    public class ClosureOutput
    {
        public PrismTypes.ClosureState ClosureState { get; set; }
        public string NarrationText { get; set; }
    }

    public class InputIntent
    {
        public string IntentID { get; set; }
        public PrismTypes.SourceSystem SourceSystem { get; set; }
        public string ActorID { get; set; }
        public PrismTypes.IntentType ExpectedIntent { get; set; }
        public int Priority { get; set; }
    }
}
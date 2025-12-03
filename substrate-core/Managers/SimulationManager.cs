using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Generators;
using substrate_core.Logging;
using substrate_core.Resolvers;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.models.Profiles;
using substrate_shared.types.models.StateMachines;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Managers
{
    public class SimulationManager
    {
        private int _tickCounter = 0;

        // Lifetime buffer of all trigger events (logged via TriggerSummary, not vb.TriggerEvents)
        private readonly List<TriggerEvent> _allEvents = new();

        public TickResult RunTick(VectorBias vb, Mood mv, NarrativeMode mode = NarrativeMode.Hybrid)
        {
            _tickCounter++;
            vb.TickId = _tickCounter;

            // Ensure collections exist for resolvers that populate summaries
            vb.Summaries ??= new Dictionary<string, ISummary>();

            ResolutionResult lastResult = default;

            // Resolve VectorBias for this tick
            var vbr = new VectorBiasResolver();
            lastResult = vbr.Resolve(vb, mv);
            DebugOverlay.LogResolver(vbr.GetType().Name, vb);

            // TODO: once all resolvers are complete, run full pipeline
            // foreach (var resolver in _resolvers) { lastResult = resolver.Resolve(vb, mv); DebugOverlay.LogResolver(resolver.GetType().Name, vb); }

            // Create the state machine first
            var stateMachine = new PersonalityStateMachine();

            // Build personality profile from updated VectorBias and state machine
            var profile = new PersonalityProfile(vb, stateMachine);
            

            // Emit PersonalitySummary into vb.Summaries for downstream consumption
            var personalitySummary = new PersonalitySummary
            {
                TickId       = vb.TickId,
                State = stateMachine.CurrentState,
                HardenedBias = stateMachine.HardenedBias,
                WoundSource  = stateMachine.WoundSource
            };
            vb.Summaries[personalitySummary.Name] = personalitySummary;

            // Acquire TriggerSummary for logging and rolling window
            var triggerSummary = TryGetSummary<TriggerSummary>(vb);
            var currentEvents  = triggerSummary?.Events ?? new List<TriggerEvent>();

            // Lifetime buffer and global event log: consume via summary only
            if (currentEvents.Count > 0)
            {
                _allEvents.AddRange(currentEvents);
                EventLog.AddEvents(currentEvents);
            }

            ValidateTickState(vb, _tickCounter, currentEvents);
            EventPrinter.PrintRollingSummary(_allEvents, _tickCounter, 5);

            // Generate narrative with mode
            var narrative = NarrativeGenerator.Generate(vb, vb.TickId, mode);

            // Build tick result and immediately clone it for immutability
            var tickResult = new TickResult
            {
                TickId        = _tickCounter,
                Bias          = vb.Clone(),                  // deep copy of bias
                Summary       = lastResult.Summary,          // last resolver’s headline summary
                Narrative     = $"[Tick {_tickCounter}] {narrative}",
                TriggerEvents = currentEvents                // sourced from TriggerSummary
            };

            return tickResult.Clone(); // ensure full deep copy snapshot
        }

        public IEnumerable<TickResult> RunSimulation(VectorBias vb, IEnumerable<Mood> moods, NarrativeMode mode = NarrativeMode.Hybrid)
        {
            var results = new List<TickResult>();
            foreach (var mood in moods)
            {
                var tickResult = RunTick(vb, mood, mode);
                results.Add(tickResult.Clone());
            }
            return results;
        }

        private static TSummary? TryGetSummary<TSummary>(VectorBias vb) where TSummary : class, ISummary
        {
            if (vb.Summaries == null || vb.Summaries.Count == 0)
                return null;

            return vb.Summaries.Values.OfType<TSummary>().FirstOrDefault();
        }

        private void ValidateTickState(VectorBias vb, int tick, IReadOnlyList<TriggerEvent> currentEvents)
        {
            // Pull persistence/volatility from summaries
            var persistenceSummary = vb.Summaries.Values.OfType<PersistenceSummary>().FirstOrDefault();
            var volatilitySummary  = vb.Summaries.Values.OfType<VolatilitySummary>().FirstOrDefault();

            var persistenceNaN = persistenceSummary == null || float.IsNaN(persistenceSummary.Current);
            var volatilityNaN  = volatilitySummary == null || float.IsNaN(volatilitySummary.Volatility);

            if (persistenceNaN || volatilityNaN)
            {
                Console.WriteLine($"[Tick {tick}] ERROR: Persistence or Volatility is NaN.");
                Console.WriteLine($"  Persistence: {(persistenceNaN ? "NaN" : persistenceSummary?.Current.ToString("F2"))}");
                Console.WriteLine($"  Volatility: {(volatilityNaN ? "NaN" : volatilitySummary?.Volatility.ToString("F2"))}");
            }

            // Trigger events come from TriggerSummary, passed in as currentEvents
            if (currentEvents != null && currentEvents.Count > 0)
            {
                Console.WriteLine($"[Tick {tick}] Trigger Events:");
                foreach (var e in currentEvents)
                {
                    var scoreText = float.IsNaN(e.Score) ? "NaN" : e.Score.ToString("F2");
                    var magText   = float.IsNaN(e.Magnitude) ? "NaN" : e.Magnitude.ToString("F2");
                    Console.WriteLine($"  {e.Type} (score={scoreText}, magnitude={magText}) — {e.Description}");
                }

                EventPrinter.PrintEventTable(currentEvents, tick);
            }
        }
    }
}
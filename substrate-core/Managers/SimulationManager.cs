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
using substrate_shared.types.structs;

namespace substrate_core.Managers
{
    public class SimulationManager
    {
        private readonly List<IResolver> _resolvers = new()
        {
            new DeltaVectorResolver(),
            new ToneClusterResolver(),
            new LegacyBiasUpdater(),
            new PersistenceResolver(),
            new TriggerResolver(),
            new VolatilityResolver(),
            new IntentActionResolver()
        };

        private int _tickCounter = 0;

        // Lifetime buffer of all trigger events
        private readonly List<TriggerEvent> _allEvents = new();

        public TickResult RunTick(VectorBias vb, Mood mv, NarrativeMode mode = NarrativeMode.Hybrid)
        {
            _tickCounter++;
            vb.TickId = _tickCounter;

            vb.TriggerEvents = new List<TriggerEvent>();
            vb.Traits ??= new List<Trait>();

            ResolutionResult lastResult = default;

            // Run through resolver pipeline
            foreach (var resolver in _resolvers)
            {
                lastResult = resolver.Resolve(vb, mv);
                vb = lastResult.Bias;
                DebugOverlay.LogResolver(resolver.GetType().Name, vb);
            }

            // Add events to lifetime buffer and global registry
            _allEvents.AddRange(vb.TriggerEvents);
            EventLog.AddEvents(vb.TriggerEvents);

            ValidateTickState(vb, _tickCounter);
            EventPrinter.PrintRollingSummary(_allEvents, _tickCounter, 5);

            // Generate narrative with mode
            var narrative = NarrativeGenerator.Generate(vb, vb.TickId, mode);

            // Build tick result and immediately clone it for immutability
            var tickResult = new TickResult
            {
                TickId        = _tickCounter,
                Bias          = vb.Clone(),   // deep copy of bias
                Summary       = lastResult.Summary,
                Narrative     = $"[Tick {_tickCounter}] {narrative}",
                TriggerEvents = vb.TriggerEvents
            };

            return tickResult.Clone(); // ensure full deep copy snapshot
        }

        public IEnumerable<TickResult> RunSimulation(VectorBias vb, IEnumerable<Mood> moods, NarrativeMode mode = NarrativeMode.Hybrid)
        {
            var results = new List<TickResult>();

            foreach (var mood in moods)
            {
                // Each tick uses the updated RunTick with mode
                var tickResult = RunTick(vb, mood, mode);

                // Clone ensures immutability of stored results
                results.Add(tickResult.Clone());
            }

            return results;
        }

        private void ValidateTickState(VectorBias vb, int tick)
        {
            bool persistenceNaN = float.IsNaN(vb.Persistence);
            bool volatilityNaN  = float.IsNaN(vb.Volatility);

            if (persistenceNaN || volatilityNaN)
            {
                Console.WriteLine($"[Tick {tick}] ERROR: Persistence or Volatility is NaN.");
                Console.WriteLine($"  Persistence: {(persistenceNaN ? "NaN" : vb.Persistence.ToString("F2"))}");
                Console.WriteLine($"  Volatility: {(volatilityNaN ? "NaN" : vb.Volatility.ToString("F2"))}");
            }

            var current = vb.TriggerEvents;
            if (current != null && current.Count > 0)
            {
                Console.WriteLine($"[Tick {tick}] Trigger Events:");
                foreach (var e in current)
                {
                    string scoreText = float.IsNaN(e.Score) ? "NaN" : e.Score.ToString("F2");
                    string magText   = float.IsNaN(e.Magnitude) ? "NaN" : e.Magnitude.ToString("F2");
                    Console.WriteLine($"  {e.Type} (score={scoreText}, magnitude={magText}) — {e.Description}");
                }

                EventPrinter.PrintEventTable(current, tick);
            }
        }
    }
}
// substrate_core/Managers/SimulationManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Generators;
using substrate_core.Resolvers;
using substrate_core.Utilities;
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
            new PersistenceResolver(),
            new ToneClusterResolver(),
            new TriggerResolver(),
            new IntentActionResolver()
        };

        private int _tickCounter = 0;

        // Lifetime buffer of all trigger events
        private readonly List<TriggerEvent> _allEvents = new();

        public string RunTick(VectorBias vb, Mood mv)
        {
            _tickCounter++;
            vb.TickId = _tickCounter;

            // Per‑tick event buffer (prevents accumulation noise)
            vb.TriggerEvents = new List<TriggerEvent>();

            vb.Traits ??= new List<Trait>();

            // Run resolvers step by step to allow debug overlays
            foreach (var resolver in _resolvers)
            {
                vb = resolver.Resolve(vb, mv);
                DebugOverlay.LogResolver(resolver.GetType().Name, vb);
            }

            // Add current tick’s events to lifetime buffer
            _allEvents.AddRange(vb.TriggerEvents);

            // Validate and print
            ValidateTickState(vb, _tickCounter);

            // Rolling summary (last 5 ticks by default)
            EventPrinter.PrintRollingSummary(_allEvents, _tickCounter, 5);

            var narrative = NarrativeGenerator.Generate(vb);
            return $"[Tick {_tickCounter}] {narrative}";
        }

        public IEnumerable<string> RunSimulation(VectorBias vb, IEnumerable<Mood> moods)
        {
            return moods.Select(mood => RunTick(vb, mood)).ToList();
        }

        private void ValidateTickState(VectorBias vb, int tick)
        {
            bool persistenceNaN = float.IsNaN(vb.Persistence);
            bool volatilityNaN = float.IsNaN(vb.ExpVolatility);

            if (persistenceNaN || volatilityNaN)
            {
                Console.WriteLine($"[Tick {tick}] ERROR: Persistence or Volatility is NaN.");
                Console.WriteLine($"  Persistence: {(persistenceNaN ? "NaN" : vb.Persistence.ToString("F2"))}");
                Console.WriteLine($"  Volatility: {(volatilityNaN ? "NaN" : vb.ExpVolatility.ToString("F2"))}");
            }

            var current = vb.TriggerEvents;
            if (current != null && current.Count > 0)
            {
                Console.WriteLine($"[Tick {tick}] Trigger Events:");
                foreach (var e in current)
                {
                    string scoreText = float.IsNaN(e.Score) ? "NaN" : e.Score.ToString("F2");
                    string magText = float.IsNaN(e.Magnitude) ? "NaN" : e.Magnitude.ToString("F2");
                    Console.WriteLine($"  {e.Type} (score={scoreText}, magnitude={magText}) — {e.Description}");
                }

                // Compact summary table for this tick
                EventPrinter.PrintEventTable(current, tick);
            }
        }
    }
}
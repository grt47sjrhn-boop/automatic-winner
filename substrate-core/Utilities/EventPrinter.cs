// substrate_core/Utilities/EventPrinter.cs
using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_core.Utilities
{
    public static class EventPrinter
    {
        public static void PrintEventTable(IEnumerable<TriggerEvent> events, int tick)
        {
            if (events == null || !events.Any())
                return;

            Console.WriteLine($"[Tick {tick}] Event Summary Table:");
            var grouped = events.GroupBy(e => e.Type)
                                .Select(g => new
                                {
                                    Type = g.Key,
                                    Count = g.Count(),
                                    AvgScore = g.Average(e => e.Score),
                                    MaxScore = g.Max(e => e.Score)
                                });

            Console.WriteLine("  Type                  Count   AvgScore   MaxScore");
            Console.WriteLine("  -------------------------------------------------");
            foreach (var g in grouped)
                Console.WriteLine($"  {g.Type,-20} {g.Count,5}   {g.AvgScore,8:F2}   {g.MaxScore,8:F2}");
        }

        // Rolling window summary across last N ticks
        public static void PrintRollingSummary(List<TriggerEvent> allEvents, int currentTick, int windowSize = 5)
        {
            if (allEvents == null || !allEvents.Any())
                return;

            int startTick = Math.Max(1, currentTick - windowSize + 1);
            var windowEvents = allEvents.Where(e => e.TickId >= startTick && e.TickId <= currentTick);

            Console.WriteLine($"[Ticks {startTick}-{currentTick}] Rolling Event Summary (last {windowSize} ticks):");

            var grouped = windowEvents.GroupBy(e => e.Type)
                                      .Select(g => new
                                      {
                                          Type = g.Key,
                                          Count = g.Count(),
                                          AvgScore = g.Average(e => e.Score),
                                          MaxScore = g.Max(e => e.Score)
                                      });

            Console.WriteLine("  Type                  Count   AvgScore   MaxScore");
            Console.WriteLine("  -------------------------------------------------");
            foreach (var g in grouped)
                Console.WriteLine($"  {g.Type,-20} {g.Count,5}   {g.AvgScore,8:F2}   {g.MaxScore,8:F2}");
        }
    }
}
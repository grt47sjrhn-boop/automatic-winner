using System.Collections.Generic;
using System.Linq;
using substrate_shared.types.structs;

namespace substrate_core.Logging
{
    public static class EventLog
    {
        private static readonly List<TriggerEvent> _events = new();

        public static void AddEvents(IEnumerable<TriggerEvent> events)
        {
            if (events == null) return;
            _events.AddRange(events);
        }

        public static IReadOnlyList<TriggerEvent> GetAllEvents() => _events;

        public static IReadOnlyList<TriggerEvent> GetRecentEvents(int windowSize)
        {
            return _events
                .OrderByDescending(e => e.TickId)
                .Take(windowSize)
                .OrderBy(e => e.TickId)
                .ToList();
        }
    }
}
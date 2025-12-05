using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.types.structs;

namespace substrate_shared.types.Summaries
{
    public class TriggerSummary : ISummary
    {
        public string Name => nameof(TriggerSummary);

        public int TickId { get; set; }
        public List<TriggerEvent> Events { get; set; } = [];
        public int Count { get; set; }

        public string Describe()
        {
            return $"[TriggerSummary] Tick={TickId}, Events={Count}, Types={string.Join(", ", Events.Select(e => e.Type))}";
        }
    }
}
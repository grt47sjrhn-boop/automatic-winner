using System.Collections.Generic;
using System.Linq;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_shared.types.models
{
    public class TickResult
    {
        public int TickId { get; set; }
        public VectorBias Bias { get; set; }
        public DeltaSummary Summary { get; set; }
        public string Narrative { get; set; }
        
        // NEW: Per-tick trigger events
        public List<TriggerEvent> TriggerEvents { get; set; } = new();
        
        public TickResult Clone()
        {
            return new TickResult
            {
                TickId        = this.TickId,
                Summary       = this.Summary,
                Narrative     = this.Narrative,
                Bias          = this.Bias?.Clone(),
                TriggerEvents = this.TriggerEvents?.Select(e => new TriggerEvent
                {
                    Type        = e.Type,
                    Description = e.Description,
                    Magnitude   = e.Magnitude,
                    Score       = e.Score,
                    TickId      = e.TickId,
                    Persistence = e.Persistence,
                    Volatility  = e.Volatility
                }).ToList() ?? new List<TriggerEvent>()
            };
        }

    }
}
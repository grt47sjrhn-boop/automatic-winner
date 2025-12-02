// substrate_shared/types/structs/TriggerEvent.cs
using substrate_shared.enums;

namespace substrate_shared.types.structs
{
    public struct TriggerEvent
    {
        public TriggerType Type { get; set; }
        public string Description { get; set; }
        public float Magnitude { get; set; }
        public float Score { get; set; }
        public int TickId { get; set; }

        // New: capture system context at event time
        public float Persistence { get; set; }
        public float Volatility { get; set; }

        public TriggerEvent(TriggerType type, string description, float magnitude, float score, int tickId, float persistence, float volatility)
        {
            Type = type;
            Description = description;
            Magnitude = magnitude;
            Score = score;
            TickId = tickId;
            Persistence = persistence;
            Volatility = volatility;
        }
    }
}
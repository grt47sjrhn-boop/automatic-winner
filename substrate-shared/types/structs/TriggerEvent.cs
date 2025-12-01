// substrate_shared/types/structs/TriggerEvent.cs
using substrate_shared.enums;

namespace substrate_shared.types.structs
{
    public struct TriggerEvent
    {
        public TriggerType Type;     // explicit type
        public string Description;   // contributor-facing narrative
        public float Magnitude;      // core magnitude (e.g., hypotenuse, weight sum)
        public float Score;          // computed score for charts/logging
        public int TickId;           // per-tick scoping
    }
}
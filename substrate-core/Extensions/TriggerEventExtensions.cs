using substrate_shared.types.structs;

namespace substrate_core.Extensions
{
    public static class TriggerEventExtensions
    {
        public static string ToNarrativeString(this TriggerEvent evt)
        {
            // Technical + narrative blend
            return $"{evt.Type}: {evt.Description} " +
                   $"[Mag {evt.Magnitude:F2}, Score {evt.Score:F2}, Tick {evt.TickId}] " +
                   $"Persistence anchored at {evt.Persistence:F2}, Volatility surged at {evt.Volatility:F2}.";
        }
    }
}
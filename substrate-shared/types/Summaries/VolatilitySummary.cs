using substrate_shared.interfaces;

namespace substrate_shared.types.Summaries
{
    public class VolatilitySummary : ISummary
    {
        public string Name => nameof(VolatilitySummary);

        // Core volatility state
        public float AccumulatedTension { get; set; }
        public float Volatility { get; set; }

        // Inputs/modifiers
        public float PersistenceBleed { get; set; }
        public float EventReleaseScore { get; set; }

        // Geometry context (for consistency with DeltaSummary)
        public float DeltaAxis { get; set; }
        public float Magnitude { get; set; }
        public float AngleTheta { get; set; }

        // Tick context
        public int TickId { get; set; }

        public string Describe()
        {
            return $"[VolatilitySummary] Tick={TickId}, " +
                   $"Tension={AccumulatedTension:F2}, Volatility={Volatility:F2}, " +
                   $"Bleed={PersistenceBleed:F2}, Release={EventReleaseScore:F2}, " +
                   $"ΔAxis={DeltaAxis:F2}, Mag={Magnitude:F2}, θ={AngleTheta:F2}";
        }
    }
}
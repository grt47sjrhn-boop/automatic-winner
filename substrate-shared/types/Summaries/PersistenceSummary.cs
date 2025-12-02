using substrate_shared.interfaces;

namespace substrate_shared.types.Summaries
{
    public class PersistenceSummary : ISummary
    {
        public string Name => nameof(PersistenceSummary);

        public float Value { get; set; }
        public float LogPressure { get; set; }
        public float ExpVolatility { get; set; }
        public float Direction { get; set; }
        public float GeometricAdj { get; set; }
        public float ErosionFactor { get; set; }
        public float AdjustedDelta { get; set; }

        public string Describe() =>
            $"Persistence={Value:F2}, Dir={Direction:+0;-0;0}, GeomAdj={GeometricAdj:F2}, " +
            $"Erosion={ErosionFactor:F2}, ΔAdj={AdjustedDelta:F2}, LogPressure={LogPressure:F2}, " +
            $"ExpVolatility={ExpVolatility:F2}";
    }
}
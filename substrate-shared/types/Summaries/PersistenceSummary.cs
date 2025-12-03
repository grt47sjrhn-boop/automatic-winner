using substrate_shared.enums;
using substrate_shared.interfaces;

namespace substrate_shared.types.Summaries
{
    /// <summary>
    /// Contributor-facing snapshot of persistence state.
    /// Combines numeric metrics (pressure, erosion) with narratable flags (trajectory, recovery).
    /// </summary>
    public class PersistenceSummary : ISummary
    {
        public string Name => nameof(PersistenceSummary);

        // Core values
        public float Current { get; set; }
        public float Previous { get; set; }
        public float Delta => Current - Previous;

        // Directional flags
        public bool IsIncreasing { get; set; }
        public bool IsDecreasing { get; set; }

        // Recovery / fracture flags
        public bool IsRecovering { get; set; }
        public bool IsFracturing { get; set; }
        public bool IsDrifting => !IsRecovering && !IsFracturing;

        // Bias trajectory marker
        public BiasTrajectory Trajectory { get; set; }

        // Recovery magnitude (if applicable)
        public float RecoveryValue { get; set; }

        // Additional metrics (from your existing summary)
        public float LogPressure { get; set; }
        public float ExpVolatility { get; set; }
        public float Direction { get; set; }
        public float GeometricAdj { get; set; }
        public float ErosionFactor { get; set; }
        public float AdjustedDelta { get; set; }

        public string Describe()
        {
            return $"[PersistenceSummary] Curr={Current:F2}, Prev={Previous:F2}, Δ={Delta:F2}, " +
                   $"Inc={IsIncreasing}, Dec={IsDecreasing}, Recovering={IsRecovering}, Fracturing={IsFracturing}, " +
                   $"Trajectory={Trajectory}, RecoveryVal={RecoveryValue:F2}, " +
                   $"GeomAdj={GeometricAdj:F2}, Erosion={ErosionFactor:F2}, ΔAdj={AdjustedDelta:F2}, " +
                   $"LogPressure={LogPressure:F2}, ExpVolatility={ExpVolatility:F2}";
        }
    }
}
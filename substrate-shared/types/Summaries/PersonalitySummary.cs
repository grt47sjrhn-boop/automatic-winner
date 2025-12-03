using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;

namespace substrate_shared.types.Summaries
{
    public class PersonalitySummary : ISummary
    {
        public string Name => nameof(PersonalitySummary);

        public int TickId { get; set; }
        public PersonalityState State { get; set; }
        public HardenedBiasType HardenedBias { get; set; }

        public float PersistenceVal { get; set; }
        public float DeltaAxis { get; set; }
        public string Trajectory { get; set; } = string.Empty;
        public float AngleTheta { get; set; }

        public bool ResilienceBonusApplied { get; set; }
        public bool ModifiersApplied { get; set; }
        public List<string> TraceLogs { get; set; } = new();
        public MoodType? WoundSource { get; set; }

        public string Describe()
        {
            var traces = TraceLogs.Count > 0 ? string.Join("; ", TraceLogs) : "none";
            return $"[PersonalitySummary] Tick={TickId}, State={State}, HardenedBias={HardenedBias}, " +
                   $"Persistence={PersistenceVal:F2}, ΔAxis={DeltaAxis:F2}, Trajectory={Trajectory}, θ={AngleTheta:F2}, " +
                   $"ResilienceBonusApplied={ResilienceBonusApplied}, ModifiersApplied={ModifiersApplied}, Traces={traces}";
        }
    }
}
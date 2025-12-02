using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.structs;

namespace substrate_shared.types.models
{
    public class VectorBias
    {
        public int TickId { get; set; }
        public int MoodAxis { get; set; }
        public TraitAffinity Legacy { get; set; }
        public float DriftMagnitude { get; set; }
        public float ResonanceScarRatio { get; set; }

        public Mood IncomingMood { get; set; }
        public Mood CurrentMood { get; set; }

        // Instead of raw floats, hold summaries
        public Dictionary<string, ISummary> Summaries { get; set; } = new();

        public T? GetSummary<T>(string key) where T : class
        {
            return Summaries.TryGetValue(key, out var summary) ? summary as T : null;
        }

        public void AddSummary(ISummary summary)
        {
            Summaries[summary.Name] = summary;
        }

        public T? GetSummary<T>() where T : class, ISummary
        {
            return Summaries.Values.OfType<T>().FirstOrDefault();
        }


        public VectorBias Clone()
        {
            return new VectorBias
            {
                TickId             = this.TickId,
                MoodAxis           = this.MoodAxis,
                Legacy             = this.Legacy,
                DriftMagnitude     = this.DriftMagnitude,
                ResonanceScarRatio = this.ResonanceScarRatio,
                IncomingMood       = this.IncomingMood,
                CurrentMood        = this.CurrentMood,
                Summaries          = new Dictionary<string, ISummary>(Summaries)
            };
        }
    }
}
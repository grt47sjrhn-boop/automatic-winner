using System.Collections.Generic;
using substrate_shared.enums;

namespace substrate_shared.types.models
{
    public class ToneResolutionResult
    {
        public Tone Baseline { get; set; }
        public Tone Blended { get; set; }
        public Tone ComplementAdjusted { get; set; }
        public Tone BiasAdjusted { get; set; }
        public Tone Final { get; set; }

        public List<string> TraceLog { get; }

        public ToneResolutionResult()
        {
            TraceLog = new List<string>();
        }

        public void AddTrace(string message)
        {
            if (!string.IsNullOrEmpty(message))
                TraceLog.Add(message);
        }
    }
}
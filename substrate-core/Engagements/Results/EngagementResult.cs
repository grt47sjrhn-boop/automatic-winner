using System;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.structs;
using substrate_shared.Registries.enums;

namespace substrate_core.Engagements.Results
{
    /// <summary>
    /// Structured output of an engagement step.
    /// Implements IResult for orchestration consistency.
    /// </summary>
    public class EngagementResult : IResult
    {
        public Guid EngagementId { get; set; } = Guid.NewGuid();
        public int Threshold { get; set; }
        public BiasDescriptor Bias { get; set; } = new BiasDescriptor();
        public FacetDistribution Shape { get; set; } = new FacetDistribution();

        /// <summary>
        /// Tone distribution keyed by ToneType (Joy, Playful, Calm, etc.).
        /// </summary>
        public Dictionary<ToneType,int> Tones { get; set; } = new();

        public string Narrative { get; set; } = string.Empty;

        public override string ToString() =>
            $"EngagementId: {EngagementId}, Threshold: {Threshold}, Bias: {Bias}, Narrative: {Narrative}";
    }
}
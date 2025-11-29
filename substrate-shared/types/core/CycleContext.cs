using System.Collections.Generic;
using substrate_shared.interfaces.core;
using substrate_shared.structs;

namespace substrate_shared.types.core
{
    public class CycleContext
    {
        public int CycleNumber { get; set; }
        public BiasVector OldBias { get; set; }
        public BiasVector NewBias { get; set; }
        public BiasDelta BiasDelta { get; set; }
        public float Magnitude { get; set; }
        public ResolutionLayers Layers { get; set; }
        public List<IArtifact> Artifacts { get; set; } = new();
        public List<ITrigger> FiredTriggers { get; set; } = new();
        public double Pressure => NewBias.HiddenPressure; // use your substrate field for pressure
    }

}
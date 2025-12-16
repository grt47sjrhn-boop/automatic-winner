using System;
using substrate_shared.DescriptorTypes.Frames;

namespace substrate_shared.Providers.Contract
{
    public interface ISimulationFrameProvider
    {
        SimulationFrame GetFrame();
    }
}
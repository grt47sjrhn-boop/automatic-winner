using System;
using substrate_shared.Descriptors.Frames;

namespace substrate_shared.Providers.Contract
{
    public interface ISimulationFrameProvider
    {
        SimulationFrame GetFrame();
    }
}
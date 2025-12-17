using System;
using System.Collections.Generic;
using substrate_shared.Descriptors.Frames;
using substrate_shared.Providers.Base;

namespace substrate_shared.Providers.Contract
{
    public interface ISimulationFrameProvider
    {
        SimulationFrame GetFrame();
        IEnumerable<IDataFrame> GenerateFrames();
    }
}
using System.Collections.Generic;
using substrate_shared.structs;

namespace substrate_shared.interfaces
{
    public interface IRunner
    {
        // Run engagement for N ticks (default 1)
        ISummary Run(int ticks = 1);

        // Finalize engagement and return polymorphic summary
        ISummary Finalize();

        // Access underlying engagement if needed
        IEngagement Engagement { get; }
    }
}
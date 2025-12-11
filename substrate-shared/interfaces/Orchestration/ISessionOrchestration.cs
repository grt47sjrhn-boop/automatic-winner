using System.Collections.Generic;
using substrate_shared.interfaces.core;
using substrate_shared.structs;

namespace substrate_shared.Orchestration
{
    /// <summary>
    /// Contract for orchestrating a full session: reset, run duels, and build report.
    /// </summary>
    public interface ISessionOrchestrator
    {
        /// <summary>
        /// Run a full session: reset, execute duels, and produce report.
        /// </summary>
        IResilienceReport RunSession(IEnumerable<(BiasVector A, BiasVector B)> duelPairs);
    }
}
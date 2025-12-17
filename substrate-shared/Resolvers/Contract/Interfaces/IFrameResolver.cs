using substrate_shared.Descriptors.Frames;
using substrate_shared.interfaces.Reports;

namespace substrate_shared.Resolvers.Contract.Interfaces
{
    public interface IFrameResolver
    {
        /// <summary>
        /// A unique name or identifier for this resolver (e.g., "IntentResolver", "ConditionResolver").
        /// </summary>
        string Name { get; }
        string Description { get; }
        string Category { get; }
        
        /// <summary>
        /// Executes resolver logic on the validated SimulationFrame.
        /// </summary>
        void Resolve(SimulationFrame input, IReportSummary report);
    }
}
using substrate_shared.Descriptors.Frames;
using substrate_shared.interfaces.Reports;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_shared.Resolvers.Contract.Types
{
    public class ContextResolver : IFrameResolver
    {
        public string Name => "ContextResolver";
        public string Category => "Context Processing";
        public string Description => "Resolves the context descriptor and applies environmental effects.";

        public void Resolve(SimulationFrame input, IReportSummary report)
        {
            var context = input.Context;
            if (context == null)
            {
                report.LogWarning("No context descriptor found.");
                return;
            }

            // TODO: Add context resolution logic here
            report.LogInfo($"ContextResolver: Resolved context '{context.Id}'.");
        }
    }
}
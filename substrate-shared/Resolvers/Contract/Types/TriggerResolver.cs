using substrate_shared.Descriptors.Frames;
using substrate_shared.interfaces.Reports;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_shared.Resolvers.Contract.Types
{
    public class TriggerResolver : IFrameResolver
    {
        public string Name => "TriggerResolver";
        public string Category => "Trigger Evaluation";
        public string Description => "Resolves trigger descriptors that initiate actions.";

        public void Resolve(SimulationFrame input, IReportSummary report)
        {
            foreach (var trigger in input.Triggers)
            {
                // TODO: Evaluate trigger logic here
                report.LogInfo($"TriggerResolver: Evaluated trigger '{trigger.Id}'.");
            }
        }
    }
}
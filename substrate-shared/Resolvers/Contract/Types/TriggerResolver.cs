using substrate_shared.DescriptorTypes.Frames;
using substrate_shared.interfaces.Reports;

namespace substrate_shared.Resolvers.Contract.Types
{
    public class TriggerResolver : IFrameResolver
    {
        public string Name => "TriggerResolver";
        public string Category => "Trigger Evaluation";
        public string Description => "Resolves trigger descriptors that initiate actions.";

        public void Resolve(SimulationFrame frame, IReportSummary report)
        {
            foreach (var trigger in frame.Triggers)
            {
                // TODO: Evaluate trigger logic here
                report.LogInfo($"TriggerResolver: Evaluated trigger '{trigger.Id}'.");
            }
        }
    }
}
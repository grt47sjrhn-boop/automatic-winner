using substrate_core.Factories;
using substrate_shared.DescriptorTypes.Frames;
using substrate_shared.interfaces.Reports;

namespace substrate_shared.Resolvers.Contract.Types
{
    public class IntentResolver : IFrameResolver
    {
        public string Name => "IntentResolver";
        public string Description => "Resolves the intent descriptor and applies tone/bias effects.";
        public string Category => "Intent Processing";

        public void Resolve(SimulationFrame frame, IReportSummary report)
        {
            var intent = frame.Intent;
            if (intent == null)
            {
                report.LogWarning($"{Name}: No intent descriptor found.");
                return;
            }

            // Build command directly from descriptor
            var command = IntentCommandFactory.Create(intent.IntentType);

            // Use factory’s DescribeAsText for enriched metadata logging
            report.LogInfo($"{Name}: Resolved intent '{intent.Id}' → " +
                           IntentCommandFactory.DescribeAsText(intent.IntentType));

            // Attach command into frame output payload for downstream consumers
            frame.OutputPayload.Add("intentCommand", command);
        }
    }
}
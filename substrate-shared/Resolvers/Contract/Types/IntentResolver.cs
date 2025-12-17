using substrate_shared.Commands;
using substrate_shared.Descriptors.Frames;
using substrate_shared.interfaces.Reports;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_shared.Resolvers.Contract.Types
{
    public class IntentResolver : IFrameResolver
    {
        public string Name => "IntentResolver";
        public string Description => "Resolves the intent descriptor and applies tone/bias effects.";
        public string Category => "Intent Processing";

        public void Resolve(SimulationFrame input, IReportSummary report)
        {
            var intent = input.Intent;
            if (intent == null)
            {
                report.LogWarning($"{Name}: No intent descriptor found.");
                return;
            }

            // Delegate to façade for full resolution (descriptor, command, logging, payload)
            IntentFacade.ResolveIntent(intent.IntentType, input, report);
        }
    }
}
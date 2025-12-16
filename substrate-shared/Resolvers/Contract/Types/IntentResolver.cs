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

            var id = string.IsNullOrWhiteSpace(intent.Id) ? "<unnamed>" : intent.Id;
            var tone = intent.IntentTone?.Label ?? "<none>";
            var bias = intent.Bias;

            report.LogInfo($"{Name}: Resolving intent '{id}' with tone '{tone}' and bias '{bias}'.");
        }
    }
}
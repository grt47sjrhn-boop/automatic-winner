using substrate_shared.DescriptorTypes.Frames;
using substrate_shared.interfaces.Reports;

namespace substrate_shared.Resolvers.Contract.Types
{
    public class ChannelResolver : IFrameResolver
    {
        public string Name => "ChannelResolver";
        public string Category => "Channel Processing";
        public string Description => "Resolves the channel descriptor and determines communication medium.";

        public void Resolve(SimulationFrame frame, IReportSummary report)
        {
            var channel = frame.Channel;
            if (channel == null)
            {
                report.LogWarning("No channel descriptor found.");
                return;
            }

            // TODO: Add channel resolution logic here
            report.LogInfo($"ChannelResolver: Resolved channel '{channel.Id}' with medium '{channel.Medium}'.");
        }
    }
}
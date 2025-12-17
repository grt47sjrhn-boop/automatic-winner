using System.Linq;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Contract;
using substrate_shared.Providers.Contract.Types;
using substrate_core.Providers.Registry;

namespace substrate_core.Internal.Engines
{
    public sealed class CatalystEngine
    {
        private readonly ServiceProviderRegistry _services;

        public CatalystEngine(ServiceProviderRegistry services)
        {
            _services = services;
        }

        public void Execute(IReportSummary report)
        {
            // Pull registered frame provider
            var frameProvider = _services.Resolve<ISimulationFrameProvider>();
            if (frameProvider == null)
            {
                report.LogError("No ISimulationFrameProvider registered.");
                return;
            }

            // Pull registered agent(s)
            var receivers = _services.ResolveAll<ISimulationFrameReceiver>().ToList();
            if (!receivers.Any())
            {
                report.LogError("No ISimulationFrameReceiver registered.");
                return;
            }

            // Generate frames and push them through agents
            foreach (var frame in frameProvider.GenerateFrames())
            {
                foreach (var receiver in receivers)
                {
                    receiver.Receive(frame, report);
                }
            }
        }
    }
}
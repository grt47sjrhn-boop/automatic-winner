using substrate_shared.Descriptors.Base;
using substrate_shared.interfaces.Reports;
using substrate_shared.Strategies;

namespace substrate_shared.Providers.Contract
{
    public interface IDescriptorStrategy<T> where T : BaseDescriptor
    {
        IntentActionStrategy Execute(T descriptor, ISimulationFrameReceiver receiver, IReportSummary report);
    }
}
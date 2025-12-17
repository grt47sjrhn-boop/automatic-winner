using substrate_shared.interfaces.Reports;
using substrate_shared.Strategies;

namespace substrate_shared.Transmitters
{
    public interface IAgentTransmitter
    {
        void Emit(IntentActionStrategy strategy, IReportSummary report);
    }
}
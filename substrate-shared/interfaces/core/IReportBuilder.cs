using substrate_shared.Reports;

namespace substrate_shared.interfaces.core
{
    /// <summary>
    /// Contract for building resilience reports from tracker and inventory data.
    /// </summary>
    public interface IReportBuilder
    {
        /// <summary>
        /// Build a resilience report using the current tracker and inventory state.
        /// </summary>
        IResilienceReport BuildReport();
    }
}
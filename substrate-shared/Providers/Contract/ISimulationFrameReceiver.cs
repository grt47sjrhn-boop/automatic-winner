using System.Collections.Generic;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Descriptors.Frames;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Base;

namespace substrate_shared.Providers.Contract
{
    public interface ISimulationFrameReceiver
    {
        /// <summary>
        /// Consume a frame and log/report its descriptors.
        /// </summary>
        void Receive(IDataFrame frame, IReportSummary report);

        /// <summary>
        /// Retrieve current counts of processed descriptor types.
        /// </summary>
        IReadOnlyDictionary<DescriptorType, int> GetCounts();
    }
}
using substrate_shared.interfaces;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Profiles;
using substrate_shared.Reports;

namespace substrate_core.Adapters
{
    /// <summary>
    /// DTO for Unity consumption.
    /// </summary>
    public class DuelResult : IDuelResult
    {
        public ISummary Summary { get; set; }
        public IResilienceReport Report { get; set; }
        public IDuelist PersistentState { get; set; }
    }
}
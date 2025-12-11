using substrate_core.Models.Profiles;
using substrate_shared.interfaces;
using substrate_shared.interfaces.core;
using substrate_shared.Reports;
using substrate_shared.Profiles;

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
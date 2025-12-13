using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Profiles;

namespace substrate_shared.interfaces.Codex
{
    /// <summary>
    /// Interface for CodexContextService, which stashes
    /// ResilienceReport, Summary, Duelist, and Inventory for engine access.
    /// </summary>
    public interface ICodexContextService : IService
    {
        IResilienceReport Report { get; }
        ISummary Summary { get; } 
        IDuelist Duelist { get; }
        IManager Inventory { get; }

        void SetReport(IResilienceReport report);
        void SetSummary(ISummary summary); 
        void SetDuelist(IDuelist duelist);
        void SetInventory(IManager inventory);

        void Clear();
    }
}
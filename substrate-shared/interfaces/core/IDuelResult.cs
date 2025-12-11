using substrate_core.Models.Profiles;
using substrate_shared.Profiles;

namespace substrate_shared.interfaces.core
{
    /// <summary>
    /// Contract for duel results consumed by Unity adapters.
    /// </summary>
    public interface IDuelResult
    {
        /// <summary>
        /// Summary of the latest duel tick.
        /// </summary>
        ISummary Summary { get; set; }

        /// <summary>
        /// Computed resilience report for the duel.
        /// </summary>
        IResilienceReport Report { get; set; }

        /// <summary>
        /// Persistent duelist state across duels.
        /// </summary>
        IDuelist PersistentState { get; set; }
    }
}
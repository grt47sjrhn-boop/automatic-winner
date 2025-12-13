using System.Collections.Generic;
using substrate_shared.interfaces.Profiles;

namespace substrate_shared.interfaces.core.Factories
{
    /// <summary>
    /// Contract for factories that create duelists.
    /// </summary>
    public interface IDuelistFactory
    {
        /// <summary>
        /// Create a random duelist with varied bias/resilience.
        /// </summary>
        IDuelist CreateRandom(string? name = null);

        /// <summary>
        /// Create a themed duelist (e.g., Joy vs Despair archetypes).
        /// </summary>
        IDuelist CreateArchetype(string archetype);

        /// <summary>
        /// Create a batch of random opponents.
        /// </summary>
        List<IDuelist> CreateBatch(int count);
    }
}
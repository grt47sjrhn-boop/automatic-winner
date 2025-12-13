namespace substrate_shared.interfaces.Codex
{
    /// <summary>
    /// Base interface for all services in the Codex system.
    /// Provides a consistent contract for lifecycle management.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Initialize the service (e.g., allocate resources).
        /// </summary>
        void Initialize();

        /// <summary>
        /// Reset the service to a clean state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Dispose or shut down the service.
        /// </summary>
        void Dispose();
    }
}
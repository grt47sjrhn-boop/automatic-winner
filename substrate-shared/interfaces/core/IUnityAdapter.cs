using substrate_shared.Reports;

namespace substrate_shared.interfaces.core
{
    /// <summary>
    /// Contract for Unity-facing adapter that runs duels and produces reports.
    /// </summary>
    public interface IUnityAdapter
    {
        IDuelResult RunDuel(int tickCount = 1, bool verbose = false, bool export = false, string exportFormat = "json");
    }
}
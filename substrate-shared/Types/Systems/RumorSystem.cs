using System;

namespace substrate_shared.Types.Systems
{
    public static class RumorSystem
    {
        public static event Action<string> OnGoodRumor;
        public static event Action<string> OnBadRumor;
        public static event Action<string> OnDiscovery;
        public static event Action<string> OnCollapse; // NEW

        public static void BroadcastGoodRumor() =>
            OnGoodRumor?.Invoke("A crew struck a rich vein!");

        public static void BroadcastBadRumor() =>
            OnBadRumor?.Invoke("A crew suffered losses in the mine...");

        public static void BroadcastDiscovery(string discovery) =>
            OnDiscovery?.Invoke($"Discovery made: {discovery}");

        public static void BroadcastCollapse(int depth) =>
            OnCollapse?.Invoke($"Collapse occurred at {depth}ft!");
    }
}
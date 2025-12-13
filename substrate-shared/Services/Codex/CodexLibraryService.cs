using System;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;

namespace substrate_shared.Services.Codex
{
    public class CodexLibraryService
    {
        /// <summary>
        /// Registers all library registries needed by Codex.
        /// Call this once during Codex initialization.
        /// </summary>
        public void Initialize()
        {
            RegisterNarrativeFlavors();
            // Future: Register other bolt-on registries here
            // RegisterCrystals();
            // RegisterTones();
            // RegisterMoods();
        }

        private void RegisterNarrativeFlavors()
        {
            foreach (NarrativeFlavor flavor in Enum.GetValues(typeof(NarrativeFlavor)))
            {
                // Each NarrativeFlavor is decorated with NarrativeAttribute
                // and aliased to a NarrativeGroup value.
                NarrativeFlavorRegistryManager.RegisterFlavor((NarrativeGroup)flavor);
            }
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
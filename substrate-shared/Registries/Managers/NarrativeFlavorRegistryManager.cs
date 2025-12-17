using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions.structs;
using substrate_shared.Registries.interfaces;

namespace substrate_shared.Registries.Managers
{
    public static class NarrativeFlavorRegistryManager
    {
        private static readonly List<IReadableRegistry> _flavors = [];

        // Register a flavor into the registry system
        public static void RegisterFlavor(NarrativeGroup group)
        {
            var entry = new RegistryValue<NarrativeGroup>(group);
            _flavors.Add(entry);
        }

        public static IEnumerable<IReadableRegistry> GetAll() => _flavors;

        public static IEnumerable<IReadableRegistry> GetByBias(Bias bias)
            => _flavors.Where(f => f.GetBias() == bias);

        public static IEnumerable<IReadableRegistry> GetByGroup(NarrativeGroup group)
            => _flavors.Where(f => f.GetGroup() == group);
    }
}

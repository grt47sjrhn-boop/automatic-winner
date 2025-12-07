using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions.structs;
using substrate_shared.Registries.interfaces;
using substrate_shared.Traits;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Registries.Managers
{
    public static class CrystalRegistryManager
    {
        private static readonly List<IReadableRegistry> _crystals = new();

        // Register a crystal into the registry system
        public static void RegisterCrystal(TraitCrystal crystal)
        {
            // Wrap the crystal type as a registry value
            var entry = new RegistryValue<CrystalType>(crystal.Type);
            _crystals.Add(entry);
        }

        public static IEnumerable<IReadableRegistry> GetAll() => _crystals;

        public static IEnumerable<TraitCrystal> GetByRarity(IEnumerable<TraitCrystal> crystals, CrystalRarity rarity)
        {
            return crystals.Where(c => c.Rarity == rarity);
        }
    }
}
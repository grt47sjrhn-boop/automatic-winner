using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions.structs;
using substrate_shared.Registries.interfaces;

namespace substrate_shared.Registries.Managers
{
    public static class RegistryManager<TEnum> where TEnum : Enum
    {
        private static readonly Dictionary<TEnum, IReadableRegistry> _cache = new();

        static RegistryManager()
        {
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                _cache[value] = new RegistryValue<TEnum>(value);
            }
        }

        // --- Basic access ---
        public static IReadableRegistry Get(TEnum value) => _cache[value];

        public static IEnumerable<IReadableRegistry> GetAll() => _cache.Values;

        // --- Cluster helpers ---
        public static IEnumerable<IReadableRegistry> GetByGroup(NarrativeGroup group)
        {
            return _cache.Values.Where(v => v.GetGroup() == group);
        }

        public static string DescribeCluster(NarrativeGroup group)
        {
            var items = GetByGroup(group).ToList();
            if (!items.Any())
                return $"No entries found for group '{group}'.";

            var header = $"Narrative Group: {group}";
            var lines = items.Select(v => $"- {v.GetDescription()} ({v.GetBias()})");
            return header + Environment.NewLine + string.Join(Environment.NewLine, lines);
        }
    }
}
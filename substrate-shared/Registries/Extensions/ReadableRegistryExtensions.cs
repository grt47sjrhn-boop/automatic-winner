using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.enums.Attributes;

namespace substrate_shared.Registries.Extensions
{
    public static class ReadableRegistryExtensions
    {
        private static Attribute? GetAttribute<TEnum>(this TEnum value) where TEnum : Enum
        {
            return value.GetType()
                        .GetField(value.ToString())?
                        .GetCustomAttributes(false)
                        .OfType<Attribute>()
                        .FirstOrDefault(a => a is NarrativeAttribute || a is RegistryNarrativeAttribute);
        }

        public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            var attr = value.GetAttribute();
            return attr switch
            {
                NarrativeAttribute na => na.Description,
                RegistryNarrativeAttribute ra => ra.Description,
                _ => value.ToString()
            };
        }

        public static Bias GetBias<TEnum>(this TEnum value) where TEnum : Enum
        {
            var attr = value.GetAttribute();
            return attr switch
            {
                NarrativeAttribute na => na.Bias,
                RegistryNarrativeAttribute ra => ra.Bias,
                _ => Bias.Neutral
            };
        }

        public static NarrativeGroup GetGroup<TEnum>(this TEnum value) where TEnum : Enum
        {
            var attr = value.GetAttribute();
            return attr switch
            {
                RegistryNarrativeAttribute ra => ra.Group,
                // For NarrativeGroup itself, the enum value *is* the group
                NarrativeAttribute => (NarrativeGroup)Enum.Parse(typeof(NarrativeGroup), value.ToString()),
                _ => NarrativeGroup.Equilibrium
            };
        }

        // --- Cluster helpers ---
        public static IEnumerable<TEnum> GetByGroup<TEnum>(NarrativeGroup group) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Where(v => v.GetGroup() == group);
        }

        public static string DescribeCluster<TEnum>(NarrativeGroup group) where TEnum : Enum
        {
            var items = GetByGroup<TEnum>(group).ToList();
            if (!items.Any())
                return $"No entries found for group '{group}'.";

            var header = $"Narrative Group: {group}";
            var lines = items.Select(v => $"- {v}: {v.GetDescription()}");
            return header + Environment.NewLine + string.Join(Environment.NewLine, lines);
        }

        // --- Axis helpers (for MoodType specifically) ---
        public static TEnum Resolve<TEnum>(float axis) where TEnum : Enum
        {
            var clamped = MathF.Max(-11f, MathF.Min(11f, axis));
            var rounded = (int)MathF.Round(clamped);

            if (Enum.IsDefined(typeof(TEnum), rounded))
                return (TEnum)(object)rounded;

            return (TEnum)(object)0; // fallback to Neutral
        }
    }
}
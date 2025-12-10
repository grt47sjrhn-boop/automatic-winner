using System;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.interfaces;

namespace substrate_shared.Registries.Extensions.structs
{
    /// <summary>
    /// Wraps an enum value in a registry-aware container, exposing bias, group, tone, and enriched description.
    /// </summary>
    public readonly struct RegistryValue<TEnum> : IReadableRegistry where TEnum : Enum
    {
        public TEnum Value { get; }

        public RegistryValue(TEnum value) => Value = value;

        /// <summary>
        /// Generate a description of the registry value, optionally enriched with overlay metrics.
        /// </summary>
        public string GetDescription(double? hypotenuse = null, double? area = null)
        {
            var baseDesc = Value.GetDescription();

            if (hypotenuse.HasValue || area.HasValue)
            {
                var hypText = hypotenuse.HasValue ? $"Hypotenuse {hypotenuse:F1}" : string.Empty;
                var areaText = area.HasValue ? $"Area {area:F1}" : string.Empty;

                var overlayInfo = string.Join(", ",
                    new[] { hypText, areaText }.Where(s => !string.IsNullOrEmpty(s)));

                if (!string.IsNullOrEmpty(overlayInfo))
                    return $"{baseDesc} [{overlayInfo}]";
            }

            return baseDesc;
        }

        public Bias GetBias() => Value.GetBias();
        public NarrativeGroup GetGroup() => Value.GetGroup();

        public ToneType GetToneType()
        {
            if (Value is ToneType tone)
                return tone;

            throw new InvalidOperationException(
                $"RegistryValue<{typeof(TEnum).Name}> does not map to ToneType."
            );
        }

        public override string ToString() => GetDescription();
    }
}
using System;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.interfaces;

namespace substrate_shared.Registries.Extensions.structs
{
    public readonly struct RegistryValue<TEnum> : IReadableRegistry where TEnum : Enum
    {
        public TEnum Value { get; }

        public RegistryValue(TEnum value) => Value = value;

        public string GetDescription() => Value.GetDescription();
        public Bias GetBias() => Value.GetBias();
        public NarrativeGroup GetGroup() => Value.GetGroup();

        public override string ToString() => GetDescription();
    }
}
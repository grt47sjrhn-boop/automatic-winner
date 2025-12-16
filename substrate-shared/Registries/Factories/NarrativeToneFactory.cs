using System;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.interfaces;

namespace substrate_shared.Registries.Factories
{
    public static class NarrativeToneFactory
    {
        // Build a NarrativeTone from any registry entry
        public static NarrativeTone FromRegistry(IReadableRegistry entry)
        {
            return new NarrativeTone(
                type: entry.GetToneType(),
                label: entry.GetDescription(),
                group: entry.GetGroup(),
                Bias.Neutral// or derive differently if you prefer
            );
        }
    }
}
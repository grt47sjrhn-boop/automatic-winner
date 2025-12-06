using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using substrate_shared.Registries.interfaces;
using substrate_shared.Registries.Base;
using substrate_shared.structs;

namespace substrate_shared.Registries.Factories
{
    public static class BiasVectorFactory
    {
        // Generic builder for any registry-backed enum
        private static BiasVector FromRegistryEntry(IReadableRegistry entry, int magnitude)
        {
            var narrativeTone = new NarrativeTone(
                type: entry.GetToneType(),
                label: entry.GetDescription(),
                category: entry.GetGroup().ToString(),
                biasValue: entry.GetBias(),
                group: entry.GetGroup().ToString()
            );

            return new BiasVector(narrativeTone, magnitude);
        }

        // ToneType → BiasVector
        public static BiasVector FromTone(ToneType tone, int magnitude)
        {
            var entry = RegistryManager<ToneType>.Get(tone);
            return FromRegistryEntry(entry, magnitude);
        }

        // MoodType → BiasVector
        public static BiasVector FromMood(MoodType mood, int magnitude)
        {
            var entry = RegistryManager<MoodType>.Get(mood);
            return FromRegistryEntry(entry, magnitude);
        }

        // IntentAction → BiasVector
        public static BiasVector FromIntent(IntentAction intent, int magnitude)
        {
            var entry = RegistryManager<IntentAction>.Get(intent);
            return FromRegistryEntry(entry, magnitude);
        }
    }
}
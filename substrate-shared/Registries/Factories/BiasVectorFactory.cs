using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using substrate_shared.Registries.interfaces;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums.Attributes;
using substrate_shared.Registries.Extensions;
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

        public static BiasVector FromToneAndMood(ToneType toneKey, MoodType moodKey, int magnitude)
        {
            // Build NarrativeTone from ToneType registry entry
            var toneEntry = RegistryManager<ToneType>.Get(toneKey);
            var tone = NarrativeToneFactory.FromRegistry(toneEntry);

            // Enrich the label with MoodType context
            var moodEntry = RegistryManager<MoodType>.Get(moodKey);
            var moodLabel = moodEntry.GetDescription();

            var enrichedTone = new NarrativeTone(
                type: tone.Type,
                label: $"{tone.Label} + {moodLabel}",
                category: tone.Category,
                biasValue: tone.BiasValue,
                group: tone.Group
            );

            return new BiasVector(enrichedTone, magnitude);
        }
        
        public static BiasVector FromAllRegistries(
            ToneType toneKey,
            MoodType moodKey,
            IntentAction intentKey,
            CrystalRarity rarityKey,
            int magnitude)
        {
            // Base NarrativeTone from ToneType
            var toneEntry = RegistryManager<ToneType>.Get(toneKey);
            var tone = NarrativeToneFactory.FromRegistry(toneEntry);

            // Pull registry descriptions
            var moodLabel   = RegistryManager<MoodType>.Get(moodKey).GetDescription();
            var intentLabel = RegistryManager<IntentAction>.Get(intentKey).GetDescription();
            var rarityLabel = RegistryManager<CrystalRarity>.Get(rarityKey).GetDescription();

            // Composite label: Tone enriched with Mood, Intent, Rarity
            var compositeLabel = $"{tone.Label} | Mood: {moodLabel} | Intent: {intentLabel} | Rarity: {rarityLabel}";

            // Build enriched NarrativeTone
            var enrichedTone = new NarrativeTone(
                type: tone.Type,
                label: compositeLabel,
                category: tone.Category,
                biasValue: tone.BiasValue,
                group: tone.Group
            );

            return new BiasVector(enrichedTone, magnitude);
        }
    }
}

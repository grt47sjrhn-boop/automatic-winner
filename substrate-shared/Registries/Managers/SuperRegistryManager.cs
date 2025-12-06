using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions.structs;
using substrate_shared.Registries.interfaces;

namespace substrate_shared.Registries.Managers
{
    public static class SuperRegistryManager
    {
        private static readonly Dictionary<Type, IEnumerable<IReadableRegistry>> _allRegistries = new();
        private static readonly Random _rng = new();

        // --- Registration ---
        public static void Register<TEnum>() where TEnum : Enum
        {
            var values = RegistryManager<TEnum>.GetAll();
            _allRegistries[typeof(TEnum)] = values;
        }

        // --- Unified sweeps ---
        public static IEnumerable<IReadableRegistry> GetAll()
            => _allRegistries.Values.SelectMany(v => v);

        public static IEnumerable<IReadableRegistry> GetByBias(Bias bias)
            => GetAll().Where(v => v.GetBias() == bias);

        public static IEnumerable<IReadableRegistry> GetByGroup(NarrativeGroup group)
            => GetAll().Where(v => v.GetGroup() == group);

        // --- Grouped breakdown ---
        public static Dictionary<Type, List<IReadableRegistry>> GetGroupedByRegistry(NarrativeGroup group)
        {
            var result = new Dictionary<Type, List<IReadableRegistry>>();
            foreach (var kvp in _allRegistries)
            {
                var filtered = kvp.Value.Where(v => v.GetGroup() == group).ToList();
                if (filtered.Any())
                {
                    result[kvp.Key] = filtered;
                }
            }
            return result;
        }

        // --- Triad overlay ---
        public static string DescribeTriad(ToneType tone, MoodType mood, IntentAction intent)
        {
            var toneEntry = RegistryManager<ToneType>.Get(tone);
            var moodEntry = RegistryManager<MoodType>.Get(mood);
            var intentEntry = RegistryManager<IntentAction>.Get(intent);

            var score = ClassifyBias(new[] { toneEntry, moodEntry, intentEntry });

            return
                $"Triad Overlay:\n" +
                $"- Tone: {toneEntry.GetDescription()} ({toneEntry.GetBias()})\n" +
                $"- Mood: {moodEntry.GetDescription()} ({moodEntry.GetBias()})\n" +
                $"- Intent: {intentEntry.GetDescription()} ({intentEntry.GetBias()})\n" +
                $"Overall Bias: {score}\n";
        }

        // --- Cluster scoring ---
        public static string DescribeClusterWithScore(NarrativeGroup group)
        {
            var items = GetByGroup(group).ToList();
            if (!items.Any())
                return $"No entries found for group '{group}'.";

            var score = ClassifyBias(items);

            var header = $"Narrative Group: {group} (Overall Bias: {score})";
            var lines = items.Select(v => $"- {v.GetDescription()} ({v.GetBias()})");
            return header + Environment.NewLine + string.Join(Environment.NewLine, lines);
        }

        // --- Bias scoring ---
        private static string ClassifyBias(IEnumerable<IReadableRegistry> entries)
        {
            var positive = entries.Count(e => e.GetBias() == Bias.Positive);
            var negative = entries.Count(e => e.GetBias() == Bias.Negative);
            var neutral  = entries.Count(e => e.GetBias() == Bias.Neutral);

            if (positive > 0 && negative > 0)
                return "Mixed (Conflict)";
            if (positive > 0 && neutral > 0 && negative == 0)
                return "Leaning Positive";
            if (negative > 0 && neutral > 0 && positive == 0)
                return "Leaning Negative";
            if (positive > 0 && negative == 0 && neutral == 0)
                return "Positive";
            if (negative > 0 && positive == 0 && neutral == 0)
                return "Negative";
            return "Neutral";
        }

        // --- Random sampling ---
        public static IReadableRegistry GetRandomEntry()
        {
            var all = GetAll().ToList();
            return all[_rng.Next(all.Count)];
        }

        public static IEnumerable<IReadableRegistry> GetRandomCluster(NarrativeGroup group, int count = 3)
        {
            var items = GetByGroup(group).ToList();
            return items.OrderBy(_ => _rng.Next()).Take(count);
        }

        // --- Narrative overlay ---
        public static string BuildOverlay(IEnumerable<IReadableRegistry> entries)
        {
            var descriptions = entries.Select(e => e.GetDescription());
            return string.Join(" | ", descriptions);
        }

        // --- Critic-facing report generator ---
        public static string GenerateCriticReport()
        {
            var report = new List<string>
            {
                "=== PRISMx Critic Report ===",
                $"Total Registries: {_allRegistries.Count}",
                $"Total Entries: {GetAll().Count()}",
                ""
            };

            // Section: Bias overview
            report.Add("Bias Overview:");
            foreach (Bias bias in Enum.GetValues(typeof(Bias)))
            {
                var count = GetByBias(bias).Count();
                report.Add($"- {bias}: {count} entries");
            }
            report.Add("");

            // Section: Group summaries
            report.Add("Group Summaries:");
            foreach (NarrativeGroup group in Enum.GetValues(typeof(NarrativeGroup)))
            {
                var summary = DescribeClusterWithScore(group);
                report.Add(summary);
                report.Add("");
            }

            // Section: Random triad sample
            var toneEntry = RegistryManager<ToneType>.GetAll().OrderBy(_ => _rng.Next()).First();
            var moodEntry = RegistryManager<MoodType>.GetAll().OrderBy(_ => _rng.Next()).First();
            var intentEntry = RegistryManager<IntentAction>.GetAll().OrderBy(_ => _rng.Next()).First();

            // Direct cast because RegistryValue<TEnum> is a struct
            var tone = ((RegistryValue<ToneType>)toneEntry).Value;
            var mood = ((RegistryValue<MoodType>)moodEntry).Value;
            var intent = ((RegistryValue<IntentAction>)intentEntry).Value;

            report.Add("Sample Triad:");
            report.Add(DescribeTriad(tone, mood, intent));

            return string.Join(Environment.NewLine, report);

        }
    }
}
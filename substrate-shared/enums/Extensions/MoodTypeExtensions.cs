using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace substrate_shared.enums.Extensions
{
    public static class MoodTypeExtensions
    {
        public static string GetDescription(this MoodType mood)
        {
            var attr = GetAttribute(mood);
            return attr?.Description ?? mood.ToString();
        }

        public static string GetValence(this MoodType mood)
        {
            var attr = GetAttribute(mood);
            return attr?.Bias ?? "Undefined";
        }

        public static string GetNarrativeGroup(this MoodType mood)
        {
            var attr = GetAttribute(mood);
            return attr?.NarrativeGroup ?? "Undefined";
        }

        public static IEnumerable<MoodType> GetByNarrativeGroup(string narrativeGroup)
        {
            return Enum.GetValues(typeof(MoodType))
                       .Cast<MoodType>()
                       .Where(m => m.GetNarrativeGroup()
                           .Equals(narrativeGroup, StringComparison.OrdinalIgnoreCase));
        }

        public static string DescribeCluster(string narrativeGroup)
        {
            var moods = GetByNarrativeGroup(narrativeGroup).ToList();
            if (!moods.Any())
                return $"No moods found for narrative group '{narrativeGroup}'.";

            var header = $"Narrative Group: {narrativeGroup}";
            var lines = moods.Select(m => $"- {m}: {m.GetDescription()}");
            return header + Environment.NewLine + string.Join(Environment.NewLine, lines);
        }

        public static string DescribeNeighborhoodCluster(float axis, int range = 2)
        {
            int rounded = (int)MathF.Round(MathF.Max(-11f, MathF.Min(11f, axis)));
            var results = new List<MoodType>();

            for (int offset = -range; offset <= range; offset++)
            {
                int candidate = rounded + offset;
                if (Enum.IsDefined(typeof(MoodType), candidate))
                    results.Add((MoodType)candidate);
            }

            var header = $"Neighborhood Cluster (axis {axis:F1}, range ±{range}):";
            var lines = results.Select(m => $"- {m}: {m.GetDescription()} (Group: {m.GetNarrativeGroup()})");
            return header + Environment.NewLine + string.Join(Environment.NewLine, lines);
        }

        public static string DescribeOpposite(float axis)
        {
            int rounded = (int)MathF.Round(MathF.Max(-11f, MathF.Min(11f, axis)));
            int opposite = -rounded;

            if (!Enum.IsDefined(typeof(MoodType), opposite))
                return $"No opposite mood found for axis {axis:F1}.";

            var oppMood = (MoodType)opposite;
            return $"Opposite of {Resolve(axis)} is {oppMood}: {oppMood.GetDescription()} (Group: {oppMood.GetNarrativeGroup()})";
        }

        public static string DescribeFullContext(float axis, int range = 2)
        {
            var current = Resolve(axis);
            string currentDesc = $"{current}: {current.GetDescription()} (Group: {current.GetNarrativeGroup()})";

            string neighborhood = DescribeNeighborhoodCluster(axis, range);
            string opposite = DescribeOpposite(axis);

            return $"Current Mood → {currentDesc}" + Environment.NewLine +
                   neighborhood + Environment.NewLine +
                   opposite;
        }

        /// <summary>
        /// Narrates the trajectory from current mood through its neighborhood toward its opposite.
        /// </summary>
        public static string DescribeTrajectory(float axis, int range = 2)
        {
            var current = Resolve(axis);
            var neighborhood = DescribeNeighborhoodCluster(axis, range);
            var opposite = DescribeOpposite(axis);

            return $"Trajectory Arc:" + Environment.NewLine +
                   $"Starting at {current} ({current.GetNarrativeGroup()}): {current.GetDescription()}" + Environment.NewLine +
                   $"Neighborhood drift:" + Environment.NewLine +
                   neighborhood + Environment.NewLine +
                   $"Potential polarity flip:" + Environment.NewLine +
                   opposite;
        }

        private static MoodDescriptionAttribute? GetAttribute(MoodType mood)
        {
            return mood.GetType()
                       .GetField(mood.ToString())?
                       .GetCustomAttribute<MoodDescriptionAttribute>();
        }

        public static MoodType Resolve(float axis)
        {
            float clamped = MathF.Max(-11f, MathF.Min(11f, axis));
            int rounded = (int)MathF.Round(clamped);

            if (Enum.IsDefined(typeof(MoodType), rounded))
                return (MoodType)rounded;

            return MoodType.Neutral;
        }
    }
}
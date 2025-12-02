using System;
using System.Collections.Generic;
using substrate_shared.enums;

namespace substrate_tools.Utilities
{
    public static class HelpGenerator
    {
        /// <summary>
        /// Generates CLI help text for the demo runner dynamically.
        /// </summary>
        public static string GenerateHelpText()
        {
            var options = new List<(string Flag, string Short, string Description, string Default)>
            {
                ("--ticks <number>", "-t", "Number of moods to generate", "20"),
                ("--hybrid <true|false>", "-h", "Use hybrid mood generator", "false"),
                ("--charts <true|false>", "-c", "Generate charts", "true"),
                ("--narratives <true|false>", "-n", "Print narrative arc", "true"),
                ("--mode <Hybrid|TechnicalOnly|NarrativeOnly>", "-m", "Narrative output mode", NarrativeMode.Hybrid.ToString()),
                ("--help", "-?", "Show this help message", "")
            };

            var helpText = @"
=== Demo Runner Help ===

Usage:
  dotnet run -- [options]

Options:
";

            foreach (var (flag, shortFlag, description, def) in options)
            {
                var defaultText = string.IsNullOrEmpty(def) ? "" : $" (default: {def})";
                helpText += $"  {flag,-30} | {shortFlag,-3} {description}{defaultText}\n";
            }

            helpText += @"
Examples:
  dotnet run -- --ticks 50 --hybrid true --charts false --mode TechnicalOnly
  dotnet run -- -t 30 -h true -c true -n false -m NarrativeOnly
";

            return helpText;
        }
    }
}
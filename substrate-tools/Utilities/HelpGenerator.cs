using System;

namespace substrate_tools.Utilities
{
    public static class HelpGenerator
    {
        /// <summary>
        /// Generates CLI help text for the demo runner.
        /// </summary>
        public static string GenerateHelpText()
        {
            return @"
=== Demo Runner Help ===

Usage:
  dotnet run -- [options]

Options:
  --ticks <number>       | -t   Number of moods to generate (default: 20)
  --hybrid <true|false>  | -h   Use hybrid mood generator (default: false)
  --charts <true|false>  | -c   Generate charts (default: true)
  --narratives <true|false> | -n   Print narrative arc (default: true)
  --help                 | -?   Show this help message

Examples:
  dotnet run -- --ticks 50 --hybrid true --charts false
  dotnet run -- -t 30 -h true -c true -n false
";
        }
    }
}
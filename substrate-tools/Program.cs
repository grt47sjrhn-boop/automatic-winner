using System;
using substrate_core;
using substrate_core.Resolvers;
using substrate_shared.interfaces;
using substrate_shared.Profiles;
using substrate_shared.Reports; // <-- bring in ResilienceReportIO

namespace substrate_tools
{
    class Program
    {
        static void Main(string[] args)
        {
            IResilienceTracker tracker = new ResilienceTracker();
            var persistent = new Duelist("Persistent Hero", initialBias: 0.0);
            var engine = new DuelEngine(tracker, persistent);

            int tickCount = 5;
            bool verbose = false;
            bool export = false;
            string exportFormat = "json";

            // Parse CLI args
            foreach (var arg in args)
            {
                if (int.TryParse(arg, out int parsed))
                {
                    tickCount = parsed;
                }
                else if (arg.Equals("--verbose", StringComparison.OrdinalIgnoreCase))
                {
                    verbose = true;
                }
                else if (arg.Equals("--export", StringComparison.OrdinalIgnoreCase))
                {
                    export = true;
                }
                else if (arg.StartsWith("--format=", StringComparison.OrdinalIgnoreCase))
                {
                    exportFormat = arg.Substring("--format=".Length);
                }
            }

            Console.WriteLine($"Starting Duel Engine with {tickCount} ticks (Verbose={verbose}, Export={export}, Format={exportFormat})...");
            engine.Tick(tickCount);
            PrintResults(tracker, persistent, verbose);

            if (export)
            {
                var report = ResilienceReportIO.Build(tracker);
                var filePath = ResilienceReportIO.SaveWithTimestamp(report, "report", exportFormat);
                Console.WriteLine($"Report exported to {filePath}");
            }

            // Interactive loop
            while (true)
            {
                Console.WriteLine("\nPress Enter to run another duel, type a number for multiple ticks, or 'exit' to quit:");
                var input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                    break;

                int count = 1;
                if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int parsedCount))
                {
                    count = parsedCount;
                }

                engine.Tick(count);
                PrintResults(tracker, persistent, verbose);

                if (export)
                {
                    var report = ResilienceReportIO.Build(tracker);
                    var filePath = ResilienceReportIO.SaveWithTimestamp(report, "report", exportFormat);
                    Console.WriteLine($"Report exported to {filePath}");
                }
            }
        }

        private static void PrintResults(IResilienceTracker tracker, Duelist persistent, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine("\n=== Duel Summaries ===");
                foreach (var summary in tracker.DuelSummaries)
                {
                    summary.Print();
                }
            }
            else
            {
                Console.WriteLine("\n=== Latest Duel Summary ===");
                tracker.DuelSummaries[^1].Print();
            }

            Console.WriteLine("\n=== Resilience Report ===");
            var report = tracker.ComputeResilience();
            report.Print();

            Console.WriteLine("\n=== Persistent Duelist State ===");
            Console.WriteLine(persistent);
        }
    }
}
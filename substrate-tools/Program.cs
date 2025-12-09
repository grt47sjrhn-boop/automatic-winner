using System;
using substrate_core;
using substrate_core.Managers;       // concrete manager implementations
using substrate_core.Resolvers;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Profiles;
using substrate_shared.Reports;      // ResilienceReportIo
using substrate_shared.Registries.enums; // ToneType, Bias, etc.

namespace substrate_tools
{
    class Program
    {
        static void Main(string[] args)
        {
            // 🔹 Initialize concrete managers
            IBiasManager biasManager       = new BiasManager();
            IFacetManager facetManager     = new FacetManager();
            IToneManager toneManager       = new ToneManager();
            IRarityManager rarityManager   = new RarityManager();
            InventoryManager inventory     = new InventoryManager();

            // 🔹 Tracker (tone/rarity managers injected)
            IResilienceTracker tracker     = new ResilienceTracker(toneManager, rarityManager);

            // Defaults
            var tickCount    = 5;
            var verbose      = false;
            var export       = false;
            var exportFormat = "json";
            double seedBias  = 0.0;
            ToneType? seedTone = null;

            // 🔹 Parse CLI args
            foreach (var arg in args)
            {
                if (int.TryParse(arg, out var parsed))
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
                else if (arg.StartsWith("--seedBias=", StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(arg.Substring("--seedBias=".Length), out var parsedBias))
                        seedBias = parsedBias;
                }
                else if (arg.StartsWith("--seed=", StringComparison.OrdinalIgnoreCase))
                {
                    var toneName = arg.Substring("--seed=".Length);
                    if (Enum.TryParse<ToneType>(toneName, true, out var parsedTone))
                        seedTone = parsedTone;
                }
            }

            // 🔹 Persistent duelist seeded with bias/tone
            var persistent = new Duelist("Persistent Hero", initialBias: seedBias);
            if (seedTone.HasValue)
            {
                persistent.SeedTone(seedTone.Value); // implement SeedTone in Duelist
            }

            // 🔹 Engine
            var engine = new DuelEngine(
                tracker,
                persistent,
                biasManager,
                facetManager,
                toneManager,
                rarityManager,
                inventory
            );

            Console.WriteLine(
                $"Starting Duel Engine with {tickCount} ticks " +
                $"(Verbose={verbose}, Export={export}, Format={exportFormat}, SeedBias={seedBias}, SeedTone={seedTone})..."
            );

            engine.Tick(tickCount);
            PrintResults(tracker, persistent, verbose, inventory);

            if (export)
            {
                ExportReport(tracker, inventory, exportFormat);
            }

            // 🔹 Interactive loop
            while (true)
            {
                Console.WriteLine("\nPress Enter to run another duel, type a number for multiple ticks, or 'exit' to quit:");
                var input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var count = 1;
                if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out var parsedCount))
                {
                    count = parsedCount;
                }

                engine.Tick(count);
                PrintResults(tracker, persistent, verbose, inventory);

                if (export)
                {
                    ExportReport(tracker, inventory, exportFormat);
                }
            }
        }

        private static void PrintResults(IResilienceTracker tracker, Duelist persistent, bool verbose, InventoryManager inventory)
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
            var report = ResilienceReportIo.Build(tracker, inventory); // ✅ use same builder
            report.Print();

            Console.WriteLine("\n=== Persistent Duelist State ===");
            Console.WriteLine(persistent);
        }

        private static void ExportReport(IResilienceTracker tracker, InventoryManager inventory, string format)
        {
            var report   = ResilienceReportIo.Build(tracker, inventory);
            var filePath = ResilienceReportIo.SaveWithTimestamp(report, "report", format);
            Console.WriteLine($"Report exported to {filePath}");
        }
    }
}
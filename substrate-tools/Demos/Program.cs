using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Generators;
using substrate_core.Managers;
using substrate_core.Logging;
using substrate_core.Resolvers;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;
using substrate_shared.interfaces;
using substrate_shared.types.Summaries;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_tools.Config;
using substrate_tools.Utilities;

namespace substrate_tools.Demos
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ParseArgs(args);

            if (config.ShowHelp)
            {
                Console.WriteLine(HelpGenerator.GenerateHelpText());
                return;
            }

            // ✅ Run audit before simulation (prints registry status)
            Console.WriteLine("=== ToneRegistry Audit ===");
            // If you have an audit helper, call it here; else leave informational header.
            Console.WriteLine("=== End Audit ===");

            var simManager = new SimulationManager();
            var vb = InitializeVectorBias();

            // === Synthetic validation ticks ===
            RunSyntheticValidation(vb);

            // Choose mood generator based on config
            var moods = config.UseHybridMoods
                ? HybridMoodGenerator.GenerateHybridSequence(config.TickCount)
                : RandomMoodGenerator.GenerateSequence(config.TickCount);

            // Run simulation
            var (persistenceValues, volatilityValues, traitWeightSeries, narratives) =
                RunSimulationWithMetrics(simManager, vb, moods, config.Mode);

            if (config.PrintNarratives)
                PrintNarratives(narratives);

            if (config.GenerateCharts)
                ChartGenerator.GenerateAllCharts(persistenceValues, volatilityValues, traitWeightSeries);
        }

        private static DemoConfig ParseArgs(string[] args)
        {
            var config = new DemoConfig();

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--help":
                    case "-?":
                        config.ShowHelp = true;
                        break;

                    case "--ticks":
                    case "-t":
                        if (TryGetArg(args, i, out var ticksArg) && int.TryParse(ticksArg, out var ticks))
                            config.TickCount = ticks;
                        break;

                    case "--hybrid":
                    case "-h":
                        if (TryGetArg(args, i, out var hybridArg) && bool.TryParse(hybridArg, out var hybrid))
                            config.UseHybridMoods = hybrid;
                        break;

                    case "--charts":
                    case "-c":
                        if (TryGetArg(args, i, out var chartsArg) && bool.TryParse(chartsArg, out var charts))
                            config.GenerateCharts = charts;
                        break;

                    case "--narratives":
                    case "-n":
                        if (TryGetArg(args, i, out var narrArg) && bool.TryParse(narrArg, out var narr))
                            config.PrintNarratives = narr;
                        break;

                    case "--mode":
                    case "-m":
                        if (TryGetArg(args, i, out var modeArg) && Enum.TryParse(modeArg, true, out NarrativeMode mode))
                            config.Mode = mode;
                        break;
                }
            }

            return config;
        }

        private static bool TryGetArg(string[] args, int index, out string value)
        {
            if (index + 1 < args.Length)
            {
                value = args[index + 1];
                return true;
            }
            value = string.Empty;
            return false;
        }

        // ✅ VectorBias initialization: summaries-first, no direct trait/event/persistence fields
        private static VectorBias InitializeVectorBias() => new VectorBias
        {
            MoodAxis = 0,
            DriftMagnitude = 0.2f,
            ResonanceScarRatio = 0.1f,
            Summaries = new Dictionary<string, ISummary>() // resolvers populate this per tick
        };

        // === Synthetic validation harness ===
        private static void RunSyntheticValidation(VectorBias vb)
        {
            var resolver = new ToneClusterResolver();

            // Joy bias
            vb.Summaries.Clear();
            vb.AddSummary(new DeltaSummary { DeltaAxis = +6.0f, AngleTheta = 0.1f, Area = +36.0f });
            vb.AddSummary(new PersistenceSummary { Current = 5.0f, ErosionFactor = 0.05f, Direction = +1 });
            resolver.Resolve(vb, new Mood());
            var joySummary = vb.Summaries.Values.OfType<ToneClusterSummary>().FirstOrDefault();
            Console.WriteLine("=== Synthetic Joy Tick ===");
            Console.WriteLine(joySummary?.Describe());

            // Despair bias
            vb.Summaries.Clear();
            vb.AddSummary(new DeltaSummary { DeltaAxis = -6.0f, AngleTheta = 3.1f, Area = -36.0f });
            vb.AddSummary(new PersistenceSummary { Current = 5.0f, ErosionFactor = 0.05f, Direction = -1 });
            resolver.Resolve(vb, new Mood());
            var despairSummary = vb.Summaries.Values.OfType<ToneClusterSummary>().FirstOrDefault();
            Console.WriteLine("=== Synthetic Despair Tick ===");
            Console.WriteLine(despairSummary?.Describe());
        }

        private static (
            List<double> persistenceValues,
            List<double> volatilityValues,
            List<IList<double>> traitWeightSeries,
            List<string> narratives
        ) RunSimulationWithMetrics(SimulationManager simManager, VectorBias vb, IEnumerable<Mood> moods, NarrativeMode mode)
        {
            var persistenceValues = new List<double>();
            var volatilityValues = new List<double>();
            var traitWeightSeries = new List<IList<double>> { new List<double>(), new List<double>(), new List<double>() };
            var narratives = new List<string>();

            // ✅ Pass mode into RunSimulation; RunTick should populate vb.Summaries and produce narrative
            var tickResults = simManager.RunSimulation(vb, moods, mode);

            foreach (var result in tickResults)
            {
                // Narrative already generated from summaries inside the tick
                narratives.Add(result.Narrative);

                // Pull persistence/volatility from summaries
                var persistenceSummary = result.Bias.Summaries.Values.OfType<PersistenceSummary>().FirstOrDefault();
                var volatilitySummary  = result.Bias.Summaries.Values.OfType<VolatilitySummary>().FirstOrDefault();

                var persistenceVal = persistenceSummary?.Current ?? double.NaN;
                var volatilityVal  = volatilitySummary?.Volatility ?? double.NaN;

                persistenceValues.Add(persistenceVal);
                volatilityValues.Add(volatilityVal);

                // Traits: contributor-facing set from TraitSummary
                var traitSummary = result.Bias.Summaries.Values.OfType<TraitSummary>().FirstOrDefault();
                var traits = traitSummary?.Traits ?? [];

                for (var i = 0; i < Math.Min(3, traits.Count); i++)
                    traitWeightSeries[i].Add(traits[i].Weight);

                // Trigger events from TriggerSummary (useful for console echo/debug)
                var triggerSummary = result.Bias.Summaries.Values.OfType<TriggerSummary>().FirstOrDefault();
                var triggerEvents = triggerSummary?.Events ?? [];

                foreach (var evt in triggerEvents)
                    Console.WriteLine($"Tick {result.TickId}: {evt.Type} — {evt.Description}");
            }

            return (persistenceValues, volatilityValues, traitWeightSeries, narratives);
        }

        private static void PrintNarratives(IEnumerable<string> narratives)
        {
            Console.WriteLine("=== Narrative Arc ===");
            foreach (var line in narratives)
                Console.WriteLine(line);
        }
    }
}
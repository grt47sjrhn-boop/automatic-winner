using System;
using System.Collections.Generic;
using substrate_core.Generators;
using substrate_core.Managers;
using substrate_shared.enums;
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

            var simManager = new SimulationManager();
            var vb = InitializeVectorBias();

            // Choose mood generator based on config
            IEnumerable<Mood> moods = config.UseHybridMoods
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

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--help":
                    case "-?":
                        config.ShowHelp = true;
                        break;
                    case "--ticks":
                    case "-t":
                        if (TryGetArg(args, i, out string ticksArg) && int.TryParse(ticksArg, out int ticks))
                            config.TickCount = ticks;
                        break;
                    case "--hybrid":
                    case "-h":
                        if (TryGetArg(args, i, out string hybridArg) && bool.TryParse(hybridArg, out bool hybrid))
                            config.UseHybridMoods = hybrid;
                        break;
                    case "--charts":
                    case "-c":
                        if (TryGetArg(args, i, out string chartsArg) && bool.TryParse(chartsArg, out bool charts))
                            config.GenerateCharts = charts;
                        break;
                    case "--narratives":
                    case "-n":
                        if (TryGetArg(args, i, out string narrArg) && bool.TryParse(narrArg, out bool narr))
                            config.PrintNarratives = narr;
                        break;
                    case "--mode":
                    case "-m":
                        if (TryGetArg(args, i, out string modeArg) && Enum.TryParse(modeArg, true, out NarrativeMode mode))
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

        private static VectorBias InitializeVectorBias() => new VectorBias
        {
            MoodAxis = 0,
            Persistence = 1.0f,
            DriftMagnitude = 0.2f,
            ResonanceScarRatio = 0.1f,
            Traits = new List<Trait>(),
            TriggerEvents = new List<TriggerEvent>()
        };

        private static (List<double> persistenceValues,
            List<double> volatilityValues,
            List<IList<double>> traitWeightSeries,
            List<string> narratives)
            RunSimulationWithMetrics(SimulationManager simManager, VectorBias vb, IEnumerable<Mood> moods, NarrativeMode mode)
        {
            var persistenceValues = new List<double>();
            var volatilityValues = new List<double>();
            var traitWeightSeries = new List<IList<double>> { new List<double>(), new List<double>(), new List<double>() };
            var narratives = new List<string>();

            // ✅ Pass mode into RunSimulation
            var tickResults = simManager.RunSimulation(vb, moods, mode);

            foreach (var result in tickResults)
            {
                // Narrative already generated in RunTick with mode,
                // but you can still regenerate if needed:
                narratives.Add(result.Narrative);

                persistenceValues.Add(result.Bias.Persistence);
                volatilityValues.Add(result.Bias.ExpVolatility);

                for (int i = 0; i < Math.Min(3, result.Bias.Traits.Count); i++)
                    traitWeightSeries[i].Add(result.Bias.Traits[i].Weight);

                foreach (var evt in result.TriggerEvents)
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
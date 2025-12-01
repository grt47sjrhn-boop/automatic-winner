using System;
using System.Collections.Generic;
using substrate_core.Managers;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_tools.Utilities;

namespace substrate_tools.Demos
{
    class Program
    {
        static void Main(string[] args)
        {
            // Show help if requested
            if (args.Length > 0 && (args[0] == "--help" || args[0] == "-?"))
            {
                Console.WriteLine(HelpGenerator.GenerateHelpText());
                return;
            }

            // Parse CLI args
            var config = ParseArgs(args);

            var simManager = new SimulationManager();
            var vb = InitializeVectorBias();

            // Choose mood generator based on config
            IEnumerable<Mood> moods = config.UseHybridMoods
                ? HybridMoodGenerator.GenerateHybridSequence(config.TickCount)
                : RandomMoodGenerator.GenerateSequence(config.TickCount);

            // Run the entire simulation sequence and collect metrics
            var (persistenceValues, volatilityValues, traitWeightSeries, narratives) =
                RunSimulationWithMetrics(simManager, vb, moods);

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
                    case "--ticks":
                    case "-t":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int ticks))
                            config.TickCount = ticks;
                        break;
                    case "--hybrid":
                    case "-h":
                        if (i + 1 < args.Length && bool.TryParse(args[i + 1], out bool hybrid))
                            config.UseHybridMoods = hybrid;
                        break;
                    case "--charts":
                    case "-c":
                        if (i + 1 < args.Length && bool.TryParse(args[i + 1], out bool charts))
                            config.GenerateCharts = charts;
                        break;
                    case "--narratives":
                    case "-n":
                        if (i + 1 < args.Length && bool.TryParse(args[i + 1], out bool narr))
                            config.PrintNarratives = narr;
                        break;
                }
            }

            return config;
        }

        private static VectorBias InitializeVectorBias()
        {
            return new VectorBias
            {
                MoodAxis = 0,
                Persistence = 1.0f,
                DriftMagnitude = 0.2f,
                ResonanceScarRatio = 0.1f,
                Traits = new List<Trait>(),
                TriggerEvents = new List<TriggerEvent>()
            };
        }

        private static (List<double> persistenceValues,
                        List<double> volatilityValues,
                        List<IList<double>> traitWeightSeries,
                        List<string> narratives)
            RunSimulationWithMetrics(SimulationManager simManager, VectorBias vb, IEnumerable<Mood> moods)
        {
            var persistenceValues = new List<double>();
            var volatilityValues = new List<double>();
            var traitWeightSeries = new List<IList<double>>();
            var narratives = new List<string>();

            var trait1 = new List<double>();
            var trait2 = new List<double>();
            var trait3 = new List<double>();

            // Run the entire simulation sequence
            var tickNarratives = simManager.RunSimulation(vb, moods);

            foreach (var narrative in tickNarratives)
            {
                narratives.Add(narrative);

                // Collect metrics after each tick
                persistenceValues.Add(vb.Persistence);
                volatilityValues.Add(vb.ExpVolatility);

                if (vb.Traits.Count >= 3)
                {
                    trait1.Add(vb.Traits[0].Weight);
                    trait2.Add(vb.Traits[1].Weight);
                    trait3.Add(vb.Traits[2].Weight);
                }
            }

            traitWeightSeries.Add(trait1);
            traitWeightSeries.Add(trait2);
            traitWeightSeries.Add(trait3);

            return (persistenceValues, volatilityValues, traitWeightSeries, narratives);
        }

        private static void PrintNarratives(IEnumerable<string> narratives)
        {
            Console.WriteLine("=== Narrative Arc ===");
            foreach (var line in narratives)
                Console.WriteLine(line);
        }
    }

    public class DemoConfig
    {
        public int TickCount { get; set; } = 20;
        public bool UseHybridMoods { get; set; } = false;
        public bool GenerateCharts { get; set; } = true;
        public bool PrintNarratives { get; set; } = true;
    }
}
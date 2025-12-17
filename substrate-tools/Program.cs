using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Internal.Engines;
using substrate_core.Providers.Registry;
using substrate_core.Startup;
using substrate_shared.Agents;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Frames;
using substrate_shared.Descriptors.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Contract;
using substrate_shared.Providers.Contract.Types;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using substrate_shared.Registries.ResolverRegistry;
using substrate_shared.Registries.StrategyRegistry;
using substrate_shared.Resolvers.Contract.Interfaces;
using substrate_shared.Strategies;
using substrate_shared.Summaries;
using substrate_shared.Transmitters;

namespace substrate_tools
{
    class Program
    {
        static void Main(string[] args)
        {
            var report = new ReportSummary();

            // Discover descriptors + resolvers
            var boot = Startup.Initialize(
                typeof(BaseDescriptor).Assembly,
                typeof(IFrameResolver).Assembly
            );

            var services = new ServiceProviderRegistry();

            // Register resolver registry
            services.Register(boot.Registry);

            // Provide a frame provider implementation
            services.Register<ISimulationFrameProvider>(new SimulationFrameProvider(new ToneManager()));

            // Register transmitter
            var transmitter = new ConsoleTransmitter();
            services.Register<IAgentTransmitter>(transmitter);

            // Register strategy registry and strategies
            var strategyRegistry = new StrategyRegistry();
            strategyRegistry.Register(new IntentStrategy(boot.Registry));
            services.Register(strategyRegistry);

            // Register agent
            var agent = new Agent("agent:alpha", strategyRegistry, transmitter);
            services.Register<ISimulationFrameReceiver>(agent);

            // CatalystEngine consumes services
            var catalyst = new CatalystEngine(services);
            catalyst.Execute(report);

            // Run through all IntentType values using resolver directly
            if (boot.Registry.TryGet<IntentDescriptor>(out var descriptor, out var resolver))
            {
                foreach (IntentAction action in Enum.GetValues(typeof(IntentAction)))
                {
                    var intentDescriptor = new IntentDescriptor { IntentType = action };
                    var frame = new SimulationFrame { Intent = intentDescriptor };
                    resolver.Resolve(frame, report);
                }
            }

            // Bias order: Positive → Neutral → Negative
            var biasOrder = new Dictionary<Bias, int>
            {
                [Bias.Positive] = 0,
                [Bias.Neutral]  = 1,
                [Bias.Negative] = 2
            };

            var groupedAndSorted =
                report.GetMessages()
                      .Select(msg => ReportSummary.ParseIntentMessage<IntentMessage>(msg))
                      .Where(x => x is not null)
                      .Select(x => x!)
                      .GroupBy(x => x.BiasValue) // group by enum
                      .OrderBy(g => biasOrder.TryGetValue(g.Key, out var ord) ? ord : int.MaxValue)
                      .SelectMany(g =>
                          g.OrderByDescending(x => x.Scale)   // primary sort: scale descending
                           .ThenBy(x => x.IntentType)         // secondary sort: intent type ascending
                           .ThenBy(x => x.Id)                 // tertiary sort: stable ordering
                      );

            foreach (var entry in groupedAndSorted)
            {
                Console.WriteLine($"[{entry.BiasValue} | Scale {entry.Scale} | Intent {entry.IntentType}] {entry.RawMessage}");
            }
        }
    }
}
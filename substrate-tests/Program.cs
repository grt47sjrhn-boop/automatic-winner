using System;
using System.Collections.Generic;
using substrate_shared.Models.InputIntent;
using substrate_shared.Models.Types;
using substrate_shared.Types;

namespace substrate_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var orchestrator = new ConstellationOrchestrator();
            var crews = new List<Crew>();
            var analyser = new TrendAnalyser();

            while (true)
            {
                Console.WriteLine("\n=== Mining Ops ===");
                Console.WriteLine("1. Recruit Crew");
                Console.WriteLine("2. Send Crew to Dig");
                Console.WriteLine("3. Review Reports");
                Console.WriteLine("4. Market Status");
                Console.WriteLine("5. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var crew = CrewFactory.CreateCrew();
                        crews.Add(crew);
                        Console.WriteLine(
                            $"Recruited {crew.Name} (Dominant Bias: {crew.BiasProfile.DominantBias}, Morale: {crew.Morale})"
                        );
                        break;

                    case "2":
                        foreach (var c in crews)
                        {
                            var closure = orchestrator.ProcessIntent(
                                new InputIntent
                                {
                                    IntentID = $"{c.Name}-Dig",
                                    SourceSystem = PrismTypes.SourceSystem.MiningOps,
                                    ActorID = c.Name,
                                    ExpectedIntent = PrismTypes.IntentType.Dig,
                                    Priority = 1
                                },
                                c.BiasProfile.DominantBias,
                                PrismTypes.TriggerID.CollapseEvent.ToString(),
                                PrismTypes.NarrationTier.Minimal.ToString()
                            );

                            Console.WriteLine($"{c.Name} report: {closure.NarrationText}");
                            c.UpdateMorale(closure);
                            analyser.ShowBiasTrends(c.BiasProfile);
                        }
                        break;

                    case "3":
                        foreach (var c in crews)
                        {
                            c.ShowStatus();
                        }
                        break;

                    case "4":
                        MarketSystem.ShowMarket();
                        break;

                    case "5":
                        return;
                }
            }
        }
    }
}
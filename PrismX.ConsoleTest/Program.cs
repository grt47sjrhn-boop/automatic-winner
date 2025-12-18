using substrate_core.Orchestration;
using substrate_shared;
using substrate_shared.Types;
using substrate_shared.Types.Enums;
using substrate_shared.Types.Modles;
using substrate_shared.Types.Systems;
using substrate_shared.Utils; // for ConsoleHelper

namespace PrismX.ConsoleTest
{
    internal static class Program
    {
        private static void Main()
        {
            var orchestrator = new ConstellationOrchestrator();
            var crews = new List<Crew>();
            var analyser = new TrendAnalyser();
            var mineTracker = new MineTrackerSystem();
            var inventory = new InventorySystem();
            var rng = new Random();
            var reports = new List<string>();
            var rumors = new List<string>();

            // Subscribe to rumor broadcasts so we can log them
            RumorSystem.OnGoodRumor += r => { rumors.Add(r); reports.Add($"Rumor: {r}"); };
            RumorSystem.OnBadRumor += r => { rumors.Add(r); reports.Add($"Rumor: {r}"); };
            RumorSystem.OnDiscovery += r => { rumors.Add(r); reports.Add($"Rumor: {r}"); };
            RumorSystem.OnCollapse += r => { rumors.Add(r); reports.Add($"Rumor: {r}"); };

            while (true)
            {
                Console.WriteLine("\n=== Mining Ops ===");
                Console.WriteLine("1. Recruit Crew");
                Console.WriteLine("2. Send Crew to Dig");
                Console.WriteLine("3. Review Reports");
                Console.WriteLine("4. InventorySystem");
                Console.WriteLine("5. Market Status");
                Console.WriteLine("6. View Rumors");
                Console.WriteLine("7. Send Crew Back");
                Console.WriteLine("8. Toggle AutoPause (Currently " + (ConsoleHelper.AutoPause ? "ON" : "OFF") + ")");
                Console.WriteLine("9. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var crew = CrewFactory.CreateCrew();
                        crews.Add(crew);
                        var recruitMsg = $"Recruited {crew.Name} (Bias: {crew.BiasProfile.DominantBias}, Morale: {crew.Morale}, Size={crew.Size})";
                        Console.WriteLine(recruitMsg);
                        reports.Add(recruitMsg);
                        ConsoleHelper.Pause();
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
                                nameof(PrismTypes.TriggerID.CollapseEvent),
                                nameof(PrismTypes.NarrationTier.Minimal)
                            );

                            var report = $"{c.Name} report: {closure.NarrationText}";
                            Console.WriteLine(report);
                            reports.Add(report);
                            ConsoleHelper.Pause();

                            c.UpdateMorale(closure.ClosureState);

                            mineTracker.AdvanceDepth(c);
                            var risk = mineTracker.CalculateCollapseRisk();
                            var depthMsg = $"{c.Name} advanced mine to {mineTracker.CurrentDepth}ft. Collapse risk: {risk}%";
                            Console.WriteLine(depthMsg);
                            reports.Add(depthMsg);
                            ConsoleHelper.Pause();

                            if (rng.NextDouble() > 0.5)
                            {
                                var discovery = rng.NextDouble() > 0.5 ? "Gold Vein" : "Ore Deposit";
                                mineTracker.AddDiscovery(discovery);
                                inventory.AddItem(discovery.Contains("Gold") ? "Gold" : "Ore", rng.Next(1, 4));
                                var discoveryMsg = $"{c.Name} discovered {discovery} at {mineTracker.CurrentDepth}ft.";
                                Console.WriteLine(discoveryMsg);
                                reports.Add(discoveryMsg);
                                RumorSystem.BroadcastDiscovery(discovery);
                                ConsoleHelper.Pause();
                            }

                            c.ShowStatus();
                            analyser.ShowBiasTrends(c.BiasProfile);
                            ConsoleHelper.Pause();

                            // End condition check
                            if (c is { Size: > 0, Morale: > 0 }) continue;
                            
                            var endMsg = $"{c.Name} has collapsed (Size={c.Size}, Morale={c.Morale}). Game Over.";
                            Console.WriteLine(endMsg);
                            reports.Add(endMsg);
                            ConsoleHelper.Pause();
                            return;
                        }
                        break;

                    case "3":
                        Console.WriteLine("\n=== Reports Log ===");
                        foreach (var r in reports) Console.WriteLine(r);
                        ConsoleHelper.Pause();
                        break;

                    case "4":
                        Console.WriteLine("\n=== InventorySystem ===");
                        inventory.ShowInventory();
                        ConsoleHelper.Pause();

                        Console.WriteLine("Enter item to sell (Gold/Ore or press Enter to skip):");
                        var item = Console.ReadLine();
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (item.Equals("Gold", StringComparison.OrdinalIgnoreCase))
                                MarketSystem.SellGold(inventory);
                            else if (item.Equals("Ore", StringComparison.OrdinalIgnoreCase))
                                MarketSystem.SellOre(inventory);
                            else
                                inventory.SellItem(item);

                            reports.Add($"Sold {item} from inventory.");
                            ConsoleHelper.Pause();
                        }
                        break;

                    case "5":
                        MarketSystem.ShowMarket();
                        ConsoleHelper.Pause();
                        break;

                    case "6":
                        Console.WriteLine("\n=== Rumors Heard ===");
                        if (rumors.Count == 0) Console.WriteLine("No rumors yet.");
                        else foreach (var r in rumors) Console.WriteLine(r);
                        ConsoleHelper.Pause();
                        break;

                    case "7":
                        Console.WriteLine("\nSelect crew to send back:");
                        for (var i = 0; i < crews.Count; i++)
                            Console.WriteLine($"{i + 1}. {crews[i].Name} (Size={crews[i].Size}, Morale={crews[i].Morale})");

                        if (int.TryParse(Console.ReadLine(), out var idx) && idx > 0 && idx <= crews.Count)
                        {
                            var retreatCrew = crews[idx - 1];
                            retreatCrew.Morale = Math.Min(100, retreatCrew.Morale + 10); // morale recovers
                            retreatCrew.Size = Math.Max(0, retreatCrew.Size - 1);       // one miner leaves
                            var retreatMsg = $"{retreatCrew.Name} retreated from the mine. Morale now {retreatCrew.Morale}, Size {retreatCrew.Size}.";
                            Console.WriteLine(retreatMsg);
                            reports.Add(retreatMsg);

                            RumorSystem.BroadcastBadRumor(); // retreat spreads caution
                            ConsoleHelper.Pause();
                        }
                        break;

                    case "8":
                        ConsoleHelper.AutoPause = !ConsoleHelper.AutoPause;
                        Console.WriteLine($"AutoPause is now {(ConsoleHelper.AutoPause ? "ON" : "OFF")}.");
                        break;

                    case "9":
                        return;
                }
            }
        }
    }
}
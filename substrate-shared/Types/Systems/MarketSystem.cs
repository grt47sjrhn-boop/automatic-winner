using System;
using substrate_shared.Utils;

namespace substrate_shared.Types.Systems;

public static class MarketSystem
{
    private static Random rng = new Random();
    private static double goldPrice = 1000.0;
    private static double orePrice = 500.0;

    // Bias modifiers for recruitment pool
    public static int GreedBiasModifier { get; internal set; } = 0;
    public static int FearBiasModifier { get; internal set; } = 0;
    public static int LoyaltyBiasModifier { get; private set; } = 0;

    static MarketSystem()
    {
        RumorSystem.OnGoodRumor += ApplyGoodRumor;
        RumorSystem.OnBadRumor += ApplyBadRumor;
        RumorSystem.OnDiscovery += ApplyDiscoveryImpact;
        RumorSystem.OnCollapse += ApplyCollapseImpact;
    }

    public static void ShowMarket()
    {
        Console.WriteLine("\n=== Market Status ===");
        Console.WriteLine($"Gold Price: {goldPrice:0.00}");
        Console.WriteLine($"Ore Price: {orePrice:0.00}");
        Console.WriteLine($"Recruitment Bias → Greed: +{GreedBiasModifier}, Fear: +{FearBiasModifier}, Loyalty: +{LoyaltyBiasModifier}");
    }

    public static void TickMarket()
    {
        goldPrice += rng.Next(-50, 51);
        orePrice += rng.Next(-30, 31);

        if (goldPrice < 500) goldPrice = 500;
        if (orePrice < 200) orePrice = 200;
    }

    // 📈 Rumor & Discovery Impacts
    private static void ApplyGoodRumor(string rumor)
    {
        Console.WriteLine($"\n[Market] Reacting to good rumor: {rumor}");
        goldPrice += rng.Next(40, 100);
        orePrice += rng.Next(20, 60);
        GreedBiasModifier += 5;
    }

    private static void ApplyBadRumor(string rumor)
    {
        Console.WriteLine($"\n[Market] Reacting to bad rumor: {rumor}");
        goldPrice -= rng.Next(20, 60);
        orePrice -= rng.Next(10, 40);
        if (goldPrice < 500) goldPrice = 500;
        if (orePrice < 200) orePrice = 200;
        FearBiasModifier += 5;
    }

    private static void ApplyDiscoveryImpact(string discovery)
    {
        Console.WriteLine($"\n[Market] Reacting to discovery: {discovery}");
        goldPrice += rng.Next(50, 120); // discovery raises demand
        orePrice += rng.Next(30, 80);
        GreedBiasModifier += 3;
        LoyaltyBiasModifier += 2;
    }

    private static void ApplyCollapseImpact(string collapseMsg)
    {
        Console.WriteLine($"\n[Market] Reacting to collapse: {collapseMsg}");
        goldPrice += rng.Next(80, 150); // scarcity spike
        orePrice += rng.Next(40, 90);
        FearBiasModifier += 10;
    }

    // 📉 Selling Impacts
    public static void SellGold(InventorySystem inventorySystem)
    {
        var goldCount = inventorySystem.GetItemCount("Gold");
        if (goldCount > 0)
        {
            var revenue = goldCount * goldPrice;
            inventorySystem.RemoveItem("Gold", goldCount);
            inventorySystem.AddCredits(revenue);

            Console.WriteLine($"Sold {goldCount} Gold for {revenue:0.00} credits.");
            Console.WriteLine("[Market] Gold price dropped due to oversupply.");
            ConsoleHelper.Pause();   // ✅ pause
        }
        else
        {
            Console.WriteLine("No Gold to sell.");
            ConsoleHelper.Pause();   // ✅ pause
        }
    }


    public static void SellOre(InventorySystem inventorySystem)
    {
        var oreCount = inventorySystem.GetItemCount("Ore");
        if (oreCount > 0)
        {
            var revenue = oreCount * orePrice;
            inventorySystem.RemoveItem("Ore", oreCount);
            inventorySystem.AddCredits(revenue);

            Console.WriteLine($"Sold {oreCount} Ore for {revenue:0.00} credits.");
            ConsoleHelper.Pause();   // ✅ pause

            // Market reacts: oversupply drops price
            orePrice -= rng.Next(10, 30);
            if (orePrice < 200) orePrice = 200;
            Console.WriteLine("[Market] Ore price dropped due to oversupply.");
            ConsoleHelper.Pause();   // ✅ pause
        }
        else
        {
            Console.WriteLine("No Ore to sell.");
            ConsoleHelper.Pause();   // ✅ pause
        }
    }
}
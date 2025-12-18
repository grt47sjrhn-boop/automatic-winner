using System;
using System.Collections.Generic;
using substrate_shared.Models.Types;
using substrate_shared.Types;
using substrate_shared.Utils;

namespace substrate_shared
{
    public class MineTracker
    {
        public int CurrentDepth { get; private set; } = 0;
        private List<string> discoveries = new List<string>();
        private Random rng = new Random();

        public void AdvanceDepth(Crew crew)
        {
            CurrentDepth += 10;

            int risk = CalculateCollapseRisk();
            int roll = rng.Next(1, 101);

            if (roll <= risk)
            {
                Console.WriteLine($"⚠ Collapse event at {CurrentDepth}ft!");

                // Morale impact first
                crew.UpdateMorale(PrismTypes.ClosureState.Conflict);

                // Roll for crew casualties
                int casualties = rng.Next(1, crew.Size + 1);
                crew.Size -= casualties;
                if (crew.Size < 0) crew.Size = 0;

                Console.WriteLine($"{crew.Name} lost {casualties} members in the collapse! Remaining size: {crew.Size}");
                ConsoleHelper.Pause();   // ✅ pause after dramatic event

                // Broadcast rumor + collapse event
                RumorSystem.BroadcastBadRumor();
                RumorSystem.BroadcastCollapse(CurrentDepth);

                // Recruitment pool impact
                MarketSystem.FearBiasModifier += 5;   // collapses spread fear
                MarketSystem.GreedBiasModifier -= 2;  // greed dampens when danger is high
                if (MarketSystem.GreedBiasModifier < 0) MarketSystem.GreedBiasModifier = 0;

                Console.WriteLine("[Recruitment] Collapse shifted bias pool: Fear ↑, Greed ↓");
                ConsoleHelper.Pause();   // ✅ pause after recruitment impact
            }
        }

        public void AddDiscovery(string discovery)
        {
            discoveries.Add(discovery);
            Console.WriteLine($"Discovery logged: {discovery} at {CurrentDepth}ft.");
            ConsoleHelper.Pause();   // ✅ pause after discovery
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Mine depth: {CurrentDepth}ft");
            Console.WriteLine($"Collapse risk: {CalculateCollapseRisk()}%");
            ConsoleHelper.Pause();   // ✅ pause after status
        }

        public int CalculateCollapseRisk()
        {
            if (CurrentDepth < 50) return 5;
            if (CurrentDepth < 100) return 15;
            if (CurrentDepth < 200) return 30;
            if (CurrentDepth < 300) return 50;
            return 75;
        }
    }
}
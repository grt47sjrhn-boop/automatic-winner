using System;
using System.Collections.Generic;
using substrate_shared.Utils;

namespace substrate_shared.Types.Systems
{
    public class InventorySystem
    {
        private Dictionary<string, int> items = new Dictionary<string, int>();
        public double Credits { get; private set; } = 0.0;

        public void AddItem(string item, int count)
        {
            if (!items.ContainsKey(item)) items[item] = 0;
            items[item] += count;
        }

        public int GetItemCount(string item) =>
            items.ContainsKey(item) ? items[item] : 0;

        public void RemoveItem(string item, int count)
        {
            if (items.ContainsKey(item))
            {
                items[item] -= count;
                if (items[item] <= 0) items.Remove(item);
            }
        }

        public void ShowInventory()
        {
            Console.WriteLine("=== InventorySystem ===");
            foreach (var kv in items)
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            Console.WriteLine($"Credits: {Credits:0.00}");
        }

        public void AddCredits(double amount)
        {
            Credits += amount;
        }

        /// <summary>
        /// Generic sell method for items not handled by MarketSystem.
        /// Uses a simple base price model.
        /// </summary>
        public void SellItem(string item)
        {
            if (!items.ContainsKey(item))
            {
                Console.WriteLine($"No {item} to sell.");
                ConsoleHelper.Pause();   // ✅ pause

                return;
            }

            var count = items[item];
            if (count <= 0)
            {
                Console.WriteLine($"No {item} to sell.");
                return;
            }

            // Simple base price model: 100 credits per item
            var basePrice = 100.0;
            var revenue = count * basePrice;

            RemoveItem(item, count);
            AddCredits(revenue);

            Console.WriteLine($"Sold {count} {item} for {revenue:0.00} credits.");
            ConsoleHelper.Pause();   // ✅ pause
        }
    }
}
using System;
using substrate_shared.Types;

namespace substrate_shared.Models.Types
{
    // === Crew Class ===
    public class Crew
    {
        public string Name { get; set; }
        public BiasProfile BiasProfile { get; set; } = new BiasProfile();
        public int Morale { get; set; } = 100;

        public int Size { get; set; } = 10;   // current miners
        public int MaxSize { get; set; } = 10; // capacity

        public Crew()
        {
            // Subscribe to bias update events
            BiasProfile.OnBiasApplied += HandleBiasApplied;
        }

        // Derived effectiveness rating (Size/MaxSize, then bias-adjusted)
        public double EffectiveRating
        {
            get
            {
                double baseRatio = MaxSize > 0 ? (double)Size / MaxSize : 0.0;
                return BiasProfile.ApplyBiasToEffectiveness(baseRatio);
            }
        }

        private void HandleBiasApplied(PrismTypes.BiasTag bias, double adjustedValue)
        {
            Console.WriteLine($"[Crew Event] {Name} bias {bias} adjusted effectiveness to {adjustedValue:0.00}");

            if (bias == PrismTypes.BiasTag.Fear && adjustedValue < 1.0)
            {
                Console.WriteLine($"{Name} hesitates due to fear bias.");
            }

            if (bias == PrismTypes.BiasTag.Greed && adjustedValue > 1.0)
            {
                RumorSystem.BroadcastGoodRumor();
            }
        }

        public void UpdateMorale(PrismTypes.ClosureState closure)
        {
            switch (closure)
            {
                case PrismTypes.ClosureState.Escalation:
                    Morale -= 30;
                    Size = Math.Max(0, Size - 2);
                    break;

                case PrismTypes.ClosureState.Conflict:
                    Morale -= 15;
                    Size = Math.Max(0, Size - 1);
                    break;

                default:
                    Morale -= 5;
                    break;
            }
        }

        public void RecruitReplacement(int count)
        {
            Size = Math.Min(MaxSize, Size + count);
            Console.WriteLine($"{Name} recruited {count} replacements. Size now {Size}/{MaxSize}.");
        }

        public void ShowStatus()
        {
            Console.WriteLine(
                $"{Name} - Size: {Size}/{MaxSize}, Morale: {Morale}, " +
                $"Dominant Bias: {BiasProfile.DominantBias} ({BiasProfile.DominantLevel:0.00}), " +
                $"Effective Rating: {EffectiveRating:0.00}"
            );
        }
    }

    // === Crew Factory ===
}
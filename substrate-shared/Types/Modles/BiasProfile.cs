using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Types.Enums;

namespace substrate_shared.Types.Modles
{
    public class BiasProfile
    {
        // Bias facets
        public double FearLevel { get; set; }
        public double GreedLevel { get; set; }
        public double LoyaltyLevel { get; set; }
        public double DistrustLevel { get; set; }

        // Derived dominant bias
        public PrismTypes.BiasTag DominantBias { get; private set; }
        public double DominantLevel { get; private set; }

        // Modifier history: reason/event → timeline of changes
        public Dictionary<string, List<(DateTime Timestamp, double Delta)>> Modifiers { get; } 
            = new Dictionary<string, List<(DateTime, double)>>();

        // Event for bias-adjusted effectiveness
        public event Action<PrismTypes.BiasTag, double> OnBiasApplied;

        public double ApplyBiasToEffectiveness(double baseRatio)
        {
            var adjusted = baseRatio;

            switch (DominantBias)
            {
                case PrismTypes.BiasTag.Greed:
                    adjusted *= 1.2;
                    break;
                case PrismTypes.BiasTag.Fear:
                    adjusted *= 0.8;
                    break;
                case PrismTypes.BiasTag.Loyalty:
                    adjusted *= 1.1;
                    break;
                case PrismTypes.BiasTag.Distrust:
                    adjusted *= 0.9;
                    break;
            }

            OnBiasApplied?.Invoke(DominantBias, adjusted);
            return adjusted;
        }

        // Apply a modifier to a given bias facet
        public void ApplyModifier(string reason, PrismTypes.BiasTag bias, double delta)
        {
            var now = DateTime.UtcNow;

            if (!Modifiers.ContainsKey(reason))
                Modifiers[reason] = new List<(DateTime, double)>();

            Modifiers[reason].Add((now, delta));

            switch (bias)
            {
                case PrismTypes.BiasTag.Fear: FearLevel += delta; break;
                case PrismTypes.BiasTag.Greed: GreedLevel += delta; break;
                case PrismTypes.BiasTag.Loyalty: LoyaltyLevel += delta; break;
                case PrismTypes.BiasTag.Distrust: DistrustLevel += delta; break;
            }

            RecalculateDominantBias();
        }

        // Recalculate dominant bias based on current levels
        public void RecalculateDominantBias()
        {
            var scores = new Dictionary<PrismTypes.BiasTag, double>
            {
                { PrismTypes.BiasTag.Fear, FearLevel },
                { PrismTypes.BiasTag.Greed, GreedLevel },
                { PrismTypes.BiasTag.Loyalty, LoyaltyLevel },
                { PrismTypes.BiasTag.Distrust, DistrustLevel }
            };

            var max = scores.OrderByDescending(kvp => kvp.Value).First();
            DominantBias = max.Key;
            DominantLevel = max.Value;
        }
    }
}
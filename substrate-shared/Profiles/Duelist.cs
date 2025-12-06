using System;
using substrate_shared.interfaces;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Factories;

namespace substrate_shared.Profiles
{
    public class Duelist
    {
        public string Name { get; }
        public double Bias { get; private set; }
        public double Resilience { get; private set; }
        public int Wounds { get; private set; }
        public int Recoveries { get; private set; }

        public Duelist(string name, double initialBias = 0.0)
        {
            Name = name;
            Bias = initialBias;
            Resilience = 1.0;
            Wounds = 0;
            Recoveries = 0;
        }

        // Apply duel outcome from EventSummary
        public void ApplyOutcome(ISummary summary)
        {
            if (summary is EventSummary eventSummary && eventSummary.Type == SummaryType.Duel)
            {
                if (eventSummary.IsResolved)
                {
                    Bias += 0.1;
                    Resilience += 0.05;
                    Recoveries++;
                }
                else
                {
                    Bias -= 0.1;
                    Resilience = Math.Max(0, Resilience - 0.05);
                    Wounds++;
                }
            }
        }

        public override string ToString()
        {
            return $"{Name} | Bias={Bias:F2}, Resilience={Resilience:F2}, Wounds={Wounds}, Recoveries={Recoveries}";
        }

        // Convert Duelist state to a BiasVector using the registry-backed ToneType and factory
        public BiasVector ToBiasVector()
        {
            // Map bias sign to a ToneType enum key (registry-backed)
            ToneType toneKey =
                Bias > 0 ? ToneType.Joy :
                Bias < 0 ? ToneType.Despairing :
                           ToneType.Neutral;

            // Scale resilience into an integer magnitude for the resolver
            int magnitude = MapResilienceToMagnitude(Resilience);

            // Delegate construction to the registry-aware BiasVectorFactory
            return BiasVectorFactory.FromTone(toneKey, magnitude);
        }

        // Clamp + scale resilience to resolver-friendly integer magnitude
        private static int MapResilienceToMagnitude(double resilience)
        {
            var clamped = Math.Max(0.0, Math.Min(resilience, 2.0)); // [0, 2]
            return (int)Math.Round(clamped * 10);                    // 0–20
        }
    }
}
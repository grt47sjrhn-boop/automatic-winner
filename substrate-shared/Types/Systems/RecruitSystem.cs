using System;
using System.Collections.Generic;
using substrate_shared.Types.Enums;
using substrate_shared.Types.Modles;

namespace substrate_shared.Types.Systems
{
    public static class RecruitSystem
    {
        private static Random rng = new Random();

        // Recruitment pool influenced by rumors
        private static List<string> archetypes = new List<string>
        {
            "Rookie", "Veteran", "Mercenary"
        };

        static RecruitSystem()
        {
            // Subscribe to rumor broadcasts
            RumorSystem.OnGoodRumor += BoostGreedyRecruits;
            RumorSystem.OnBadRumor += BoostFearfulRecruits;
            RumorSystem.OnDiscovery += BoostLoyalRecruits;
        }

        public static Crew RecruitCrew()
        {
            var crew = CrewFactory.CreateCrew();

            // Assign archetype based on bias
            crew.Name += $" ({PickArchetype(crew.BiasProfile.DominantBias)})";

            Console.WriteLine($"Recruited {crew.Name} with dominant bias {crew.BiasProfile.DominantBias}");
            return crew;
        }

        private static string PickArchetype(PrismTypes.BiasTag bias)
        {
            return bias switch
            {
                PrismTypes.BiasTag.Greed => "Rookie",
                PrismTypes.BiasTag.Fear => "Veteran",
                PrismTypes.BiasTag.Distrust => "Mercenary",
                PrismTypes.BiasTag.Loyalty => "Veteran",
                _ => archetypes[rng.Next(archetypes.Count)]
            };
        }

        // Rumor reactions
        private static void BoostGreedyRecruits(string rumor)
        {
            Console.WriteLine($"[Recruitment] Rumor '{rumor}' → More rookies available (Greed bias).");
        }

        private static void BoostFearfulRecruits(string rumor)
        {
            Console.WriteLine($"[Recruitment] Rumor '{rumor}' → Veterans more likely to enlist (Fear bias).");
        }

        private static void BoostLoyalRecruits(string discovery)
        {
            Console.WriteLine($"[Recruitment] Discovery '{discovery}' → Loyalty bias strengthened, veterans stay committed.");
        }
    }
}
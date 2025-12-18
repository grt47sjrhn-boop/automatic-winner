using System;
using substrate_shared.Types.Systems;

namespace substrate_shared.Types.Modles
{
    public static class CrewFactory
    {
        private static Random rng = new Random();

        // Bias modifiers tracked locally for recruitment pool
        private static int fearBiasModifier = 0;
        private static int greedBiasModifier = 0;
        private static int loyaltyBiasModifier = 0;

        static CrewFactory()
        {
            // Subscribe to rumor/discovery/collapse broadcasts
            RumorSystem.OnGoodRumor += HandleGoodRumor;
            RumorSystem.OnBadRumor += HandleBadRumor;
            RumorSystem.OnDiscovery += HandleDiscovery;
            RumorSystem.OnCollapse += HandleCollapse;
        }

        private static void HandleGoodRumor(string msg)
        {
            Console.WriteLine($"[Recruitment] Good rumor heard: {msg}. Greed bias rising in new recruits.");
            greedBiasModifier += 2;
        }

        private static void HandleBadRumor(string msg)
        {
            Console.WriteLine($"[Recruitment] Bad rumor heard: {msg}. Fear bias rising in new recruits.");
            fearBiasModifier += 2;
        }

        private static void HandleDiscovery(string msg)
        {
            Console.WriteLine($"[Recruitment] Discovery heard: {msg}. Loyalty bias rising in new recruits.");
            loyaltyBiasModifier += 2;
        }

        private static void HandleCollapse(string msg)
        {
            Console.WriteLine($"[Recruitment] Collapse heard: {msg}. Fear bias strongly rising in new recruits.");
            fearBiasModifier += 5;
            greedBiasModifier -= 2; // collapses dampen greed
            if (greedBiasModifier < 0) greedBiasModifier = 0;
        }

        public static Crew CreateCrew()
        {
            // Base crew size before modifiers
            var baseSize = 10;

            // Fear reduces available recruits, Greed increases them
            var sizeModifier = 0;
            sizeModifier -= fearBiasModifier / 2;   // every 2 fear points reduces crew size by 1
            sizeModifier += greedBiasModifier / 2;  // every 2 greed points increases crew size by 1

            var finalSize = baseSize + sizeModifier;

            // Clamp to sensible bounds
            if (finalSize < 2) finalSize = 2;   // minimum crew size
            if (finalSize > 20) finalSize = 20; // maximum crew size

            var crew = new Crew
            {
                Name = $"Crew-{Guid.NewGuid().ToString().Substring(0, 4)}",
                Morale = 100,
                Size = finalSize
            };

            // Apply recruitment pool bias modifiers
            crew.BiasProfile.FearLevel += fearBiasModifier;
            crew.BiasProfile.GreedLevel += greedBiasModifier;
            crew.BiasProfile.LoyaltyLevel += loyaltyBiasModifier;

            // Randomize base bias levels for variety
            crew.BiasProfile.FearLevel += rng.Next(0, 10);
            crew.BiasProfile.GreedLevel += rng.Next(0, 10);
            crew.BiasProfile.LoyaltyLevel += rng.Next(0, 10);
            crew.BiasProfile.DistrustLevel += rng.Next(0, 10);

            crew.BiasProfile.RecalculateDominantBias();

            Console.WriteLine($"[Recruitment] Crew created with {crew.Size} members (Fear={fearBiasModifier}, Greed={greedBiasModifier}, Loyalty={loyaltyBiasModifier}).");

            return crew;
        }
    }
}
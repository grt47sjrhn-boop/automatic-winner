namespace substrate_shared.interfaces.Profiles.Types
{
    /// <summary>
    /// Static registry of predefined opponent profiles.
    /// </summary>
    public static class OpponentProfiles
    {
        public static IOpponentProfile StoryMode => new OpponentProfile
        {
            CollapseBiasFactor   = 1.2,
            RecoveryBiasFactor   = 1.0,
            NeutralBiasFactor    = 1.1,
            MagnitudeVariance    = 0.7,
            DifficultyMultiplier = 0.9,
            AggressionNudge      = false,
            Label                = "Duel",
            Category             = "Story"
        };

        public static IOpponentProfile Challenge => new OpponentProfile
        {
            CollapseBiasFactor   = 1.8,
            RecoveryBiasFactor   = 0.7,
            NeutralBiasFactor    = 0.9,
            MagnitudeVariance    = 1.3,
            DifficultyMultiplier = 1.4,
            AggressionNudge      = true,
            Label                = "Duel",
            Category             = "Challenge"
        };

        public static IOpponentProfile Nightmare => new OpponentProfile
        {
            CollapseBiasFactor   = 2.4,
            RecoveryBiasFactor   = 0.6,
            NeutralBiasFactor    = 0.8,
            MagnitudeVariance    = 1.8,
            DifficultyMultiplier = 1.8,
            AggressionNudge      = true,
            Label                = "Duel",
            Category             = "Nightmare"
        };

        public static IOpponentProfile Balanced => new OpponentProfile
        {
            CollapseBiasFactor   = 1.0,
            RecoveryBiasFactor   = 1.0,
            NeutralBiasFactor    = 0.2,
            MagnitudeVariance    = 1.2,
            DifficultyMultiplier = 1.0,
            AggressionNudge      = true,
            Label                = "Duel",
            Category             = "Balanced"
        };
    }
}
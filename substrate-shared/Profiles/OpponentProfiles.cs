namespace substrate_shared.Profiles
{
    public static class OpponentProfiles
    {
        public static OpponentProfile StoryMode => new()
        {
            CollapseBiasFactor   = 1.2,   // collapse tones slightly favored
            RecoveryBiasFactor   = 1.0,   // recovery tones balanced
            NeutralBiasFactor    = 1.1,   // neutrals show up a bit more
            MagnitudeVariance    = 0.7,   // calmer magnitudes
            DifficultyMultiplier = 0.9,   // easier duels
            AggressionNudge      = false,
            Label                = "Duel",
            Category             = "Story"
        };

        public static OpponentProfile Challenge => new()
        {
            CollapseBiasFactor   = 1.8,   // collapse tones strongly favored
            RecoveryBiasFactor   = 0.7,
            NeutralBiasFactor    = 0.9,
            MagnitudeVariance    = 1.3,   // more variance
            DifficultyMultiplier = 1.4,   // harder duels
            AggressionNudge      = true,
            Label                = "Duel",
            Category             = "Challenge"
        };

        public static OpponentProfile Nightmare => new()
        {
            CollapseBiasFactor   = 2.4,   // collapse tones dominate
            RecoveryBiasFactor   = 0.6,
            NeutralBiasFactor    = 0.8,
            MagnitudeVariance    = 1.8,   // wild swings
            DifficultyMultiplier = 1.8,   // brutal difficulty
            AggressionNudge      = true,
            Label                = "Duel",
            Category             = "Nightmare"
        };
    }
    public class OpponentProfile
    {
        public double CollapseBiasFactor   { get; set; } = 1.5;
        public double RecoveryBiasFactor   { get; set; } = 0.8;
        public double NeutralBiasFactor    { get; set; } = 1.0;
        public double MagnitudeVariance    { get; set; } = 1.0;
        public double DifficultyMultiplier { get; set; } = 1.0;
        public bool AggressionNudge        { get; set; } = true;
        public string? Label               { get; set; } = null;
        public string? Category            { get; set; } = null;
    }
}
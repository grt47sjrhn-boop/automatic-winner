namespace substrate_shared.Profiles
{
    public class OpponentProfile : IOpponentProfile
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
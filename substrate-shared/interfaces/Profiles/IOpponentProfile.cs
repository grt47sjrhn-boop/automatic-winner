using substrate_core.Models.Profiles;

namespace substrate_shared.Profiles
{
    public interface IOpponentProfile : IProfile
    {
        double CollapseBiasFactor   { get; set; }
        double RecoveryBiasFactor   { get; set; }
        double NeutralBiasFactor    { get; set; }
        double MagnitudeVariance    { get; set; }
        double DifficultyMultiplier { get; set; }
        bool AggressionNudge        { get; set; }
        string? Label               { get; set; }
        string? Category            { get; set; }
    }
}
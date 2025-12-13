// substrate_shared/DTO/Codex/ResilienceReportView.cs
namespace substrate_shared.DTO.Codex
{
    public sealed class ResilienceReportView
    {
        public string ProfileLineage { get; }
        public string Bias { get; }
        public string Summary { get; }

        public ResilienceReportView(string profileLineage, string bias, string summary)
        {
            ProfileLineage = profileLineage;
            Bias = bias;
            Summary = summary;
        }
    }
}
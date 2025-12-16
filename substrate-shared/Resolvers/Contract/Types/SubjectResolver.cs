using substrate_shared.Descriptors.Frames;
using substrate_shared.interfaces.Reports;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_shared.Resolvers.Contract.Types
{
    public class SubjectResolver : IFrameResolver
    {
        public string Name => "SubjectResolver";
        public string Category => "Subject Linking";
        public string Description => "Resolves subject descriptors and links them to context or intent.";

        public void Resolve(SimulationFrame frame, IReportSummary report)
        {
            foreach (var subject in frame.Subjects)
            {
                // TODO: Add subject linking logic here
                report.LogInfo($"SubjectResolver: Resolved subject '{subject.Id}'.");
            }
        }
    }
}
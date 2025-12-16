using substrate_shared.Descriptors.Frames;
using substrate_shared.interfaces.Reports;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_shared.Resolvers.Contract.Types;

public class ResultResolver : IFrameResolver
{
    public string Name => "ResultResolver";
    public string Category => "Result Processing";
    public string Description => "Resolves result descriptors and determines outcomes.";

    public void Resolve(SimulationFrame frame, IReportSummary report)
    {
        if (frame.Results == null || frame.Results.Count == 0)
        {
            report.LogWarning("No result descriptors found.");
            return;
        }

        foreach (var result in frame.Results)
        {
            var label = result.ResolutionLabel ?? "Unlabeled";
            var formula = result.ResultFormula?.Id ?? "None";
            var bias = result.Bias.ToString();

            report.LogInfo($"ResultResolver: Processed result '{result.Id}' → Label: '{label}', Formula: '{formula}', Bias: '{bias}'.");
        }
    }
}
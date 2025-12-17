using System;
using substrate_shared.interfaces.Reports;
using substrate_shared.Strategies;
using substrate_shared.Transmitters;

public sealed class ConsoleTransmitter : IAgentTransmitter
{
    public void Emit(IntentActionStrategy strategy, IReportSummary report)
    {
        if (strategy == null)
        {
            Console.WriteLine("[TX] Null strategy passed to transmitter.");
            return;
        }

        // Increment counters or metrics based on descriptor type
        report.Increment(strategy.Type);

        // Emit to console for visibility
        var toneText = strategy.Tone?.ToString() ?? "None";
        var narrativeText = strategy.Narrative ?? string.Empty;

        Console.WriteLine(
            $"[TX] {strategy.Type} ({narrativeText}) emitted with ActionId={strategy.ActionId}, Tone={toneText}"
        );
    }
}
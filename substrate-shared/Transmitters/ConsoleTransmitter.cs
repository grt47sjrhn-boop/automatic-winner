using System;
using substrate_shared.interfaces.Reports;
using substrate_shared.Strategies;

namespace substrate_shared.Transmitters
{
    public sealed class ConsoleTransmitter : IAgentTransmitter
    {
        // Canonical Emit method using IReportSummary
        public void Emit(IntentActionStrategy strategy, IReportSummary report)
        {
            // Increment counters or metrics based on descriptor type
            report.Increment(strategy.Type);

            // Emit to console for visibility
            Console.WriteLine(
                $"[TX] {strategy.Type} emitted with ActionId={strategy.ActionId}, Tone={strategy.Tone}"
            );
        }
    }
}
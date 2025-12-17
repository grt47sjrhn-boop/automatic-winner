using System.Collections.Generic;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Descriptors.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Base;
using substrate_shared.Providers.Contract;
using substrate_shared.Registries.StrategyRegistry;
using substrate_shared.Transmitters;

namespace substrate_shared.Agents;

public sealed class Agent : ISimulationFrameReceiver
{
    private readonly StrategyRegistry _strategies;
    private readonly IAgentTransmitter _tx;
    private readonly List<IDataFrame> _history = new();
    private readonly Dictionary<DescriptorType, int> _counters = new();

    public string Id { get; }

    public Agent(string id, StrategyRegistry strategies, IAgentTransmitter tx)
    {
        Id = id;
        _strategies = strategies;
        _tx = tx;
    }

    // Canonical Receive method that satisfies ISimulationFrameReceiver
    public void Receive(IDataFrame frame, IReportSummary report)
    {
        _history.Add(frame);

        foreach (var d in frame.GetDescriptors())
        {
            var stratObj = _strategies.Get(d.GetType());
            if (stratObj is null)
            {
                report.LogWarning($"No strategy registered for {d.Type}");
                continue;
            }

            if (d is IntentDescriptor intent && stratObj is IDescriptorStrategy<IntentDescriptor> si)
            {
                var resolved = si.Execute(intent, this, report);
                _tx.Emit(resolved, report);

                // Increment both agent-local and report counters
                if (_counters.ContainsKey(intent.Type))
                    _counters[intent.Type]++;
                else
                    _counters[intent.Type] = 1;

                report.Increment(intent.Type);
            }
            else
            {
                report.LogWarning($"Unsupported descriptor type {d.GetType().Name}");
            }
        }
    }

    /// <summary>
    /// Retrieve current counts of processed descriptor types.
    /// </summary>
    public IReadOnlyDictionary<DescriptorType, int> GetCounts() => _counters;

    public IReadOnlyList<IDataFrame> History => _history;
}
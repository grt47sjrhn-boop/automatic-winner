using System.Collections.Generic;
using substrate_shared.Providers.Contract;

namespace substrate_shared.Registries.Agent
{
    public sealed class AgentRegistry
    {
        private readonly Dictionary<string, ISimulationFrameReceiver> _agents = new();

        public void Register(string id, ISimulationFrameReceiver agent) => _agents[id] = agent;
        public ISimulationFrameReceiver? Get(string id) => _agents.TryGetValue(id, out var a) ? a : null;
        public IEnumerable<ISimulationFrameReceiver> All() => _agents.Values;
    }
}
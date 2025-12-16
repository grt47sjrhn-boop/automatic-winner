using System.Collections.Generic;
using substrate_shared.Registries.enums;

namespace substrate_shared.interfaces.Commands
{
    public interface ICommand
    {
        // Every command must expose its identity
        IntentAction Action { get; }
        string Narrative { get; }
        Bias Bias { get; }
        NarrativeGroup Group { get; }
        int ScaleValue { get; }

        // Behavior hook
        void Execute();

        // Snapshot for downstream summaries
        Dictionary<string, object> Describe();
    }
}
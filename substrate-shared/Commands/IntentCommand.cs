using System;
using System.Collections.Generic;
using substrate_shared.interfaces.Commands;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;

namespace substrate_shared.Commands
{
    public class IntentCommand : ICommand
    {
        public IntentAction Action { get; }
        public string Narrative { get; }
        public Bias Bias { get; }
        public NarrativeGroup Group { get; }
        public int ScaleValue { get; }

        public IntentCommand(IntentAction action)
        {
            Action     = action;
            Narrative  = SuperRegistryManager.GetDescription(action);
            Bias       = SuperRegistryManager.GetBias(action);
            Group      = SuperRegistryManager.GetGroup(action);
            ScaleValue = SuperRegistryManager.GetScaleValue(action);
        }

        public void Execute()
        {
            Console.WriteLine($"Executing {Action} with bias {Bias} and scale {ScaleValue}");
        }

        public Dictionary<string, object> Describe() => new()
        {
            { "intentAction", Action },
            { "bias", Bias },
            { "group", Group },
            { "scaleValue", ScaleValue },
            { "narrative", Narrative }
        };
    }
}
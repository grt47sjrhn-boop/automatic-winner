using System;
using substrate_shared.Descriptors.Frames;
using substrate_shared.Descriptors.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;

namespace substrate_shared.Commands;

public static class IntentFacade
{
    public static IntentCommand ResolveIntent(IntentAction action, SimulationFrame frame, IReportSummary report)
    {
        // Build descriptor using registry lookups
        frame.Intent = new IntentDescriptor
        {
            Id = Guid.NewGuid().ToString(),
            IntentType = action,
            Bias = SuperRegistryManager.GetBias(action),
            Group = SuperRegistryManager.GetGroup(action),
            ScaleValue = SuperRegistryManager.GetScaleValue(action),
            Narrative = SuperRegistryManager.GetDescription(action)
        };

        // Delegate command creation to the factory
        var command = IntentCommandFactory.Create(action);

        // Log narratable description via factory
        report.LogInfo(IntentCommandFactory.DescribeAsText(action));

        // Attach command to payload
        frame.OutputPayload.Add("intentCommand", command);

        return command;
    }
}
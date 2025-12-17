using System;
using substrate_shared.Descriptors.Frames;
using substrate_shared.Descriptors.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;

namespace substrate_shared.Commands
{
    public static class IntentFacade
    {
        public static IntentCommand ResolveIntent(IntentAction action, SimulationFrame frame, IReportSummary report)
        {
            // Build descriptor using registry lookups
            var descriptor = new IntentDescriptor
            {
                Id          = Guid.NewGuid().ToString(),
                IntentType  = action,
                Bias        = SuperRegistryManager.GetBias(action),
                Group       = SuperRegistryManager.GetGroup(action),
                ScaleValue  = SuperRegistryManager.GetScaleValue(action),
                Narrative   = SuperRegistryManager.GetDescription(action)
            };

            frame.Intent = descriptor;

            // Create command via factory
            var command = IntentCommandFactory.Create(action);

            // Log full narratable description
            var description = IntentCommandFactory.DescribeAsText(action);
            report.LogInfo(
                $"IntentCommand: {action} | Narrative: {descriptor.Narrative} | Bias: {descriptor.Bias} | Group: {descriptor.Group} | Scale: {descriptor.ScaleValue}"
            );

            // Attach command to payload
            frame.OutputPayload.Add("intentCommand", command);

            return command;
        }
    }
}
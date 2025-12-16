using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Commands;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;

namespace substrate_core.Factories
{
    public static class IntentCommandFactory
    {
        /// <summary>
        /// Build a new IntentCommand for the given action.
        /// </summary>
        public static IntentCommand Create(IntentAction action)
        {
            return new IntentCommand(action);
        }

        /// <summary>
        /// Build and immediately describe the command as a dictionary.
        /// </summary>
        public static Dictionary<string, object> Describe(IntentAction action)
        {
            var command = Create(action);
            return command.Describe();
        }

        /// <summary>
        /// Build and return a human-readable description string.
        /// </summary>
        public static string DescribeAsText(IntentAction action)
        {
            var command = Create(action);
            return $"IntentCommand: {command.Action} | " +
                   $"Narrative: {command.Narrative} | " +
                   $"Bias: {command.Bias} | " +
                   $"Group: {command.Group} | " +
                   $"Scale: {command.ScaleValue}";
        }

        /// <summary>
        /// Build multiple commands for a cluster of actions.
        /// </summary>
        public static IEnumerable<IntentCommand> CreateCluster(IEnumerable<IntentAction> actions)
        {
            return actions.Select(Create);
        }

        /// <summary>
        /// Describe a cluster of actions as text.
        /// </summary>
        public static string DescribeCluster(IEnumerable<IntentAction> actions)
        {
            var commands = CreateCluster(actions);
            var lines = new List<string>();

            foreach (var cmd in commands)
            {
                lines.Add($"- {cmd.Action}: {cmd.Narrative} ({cmd.Bias}, Scale {cmd.ScaleValue})");
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
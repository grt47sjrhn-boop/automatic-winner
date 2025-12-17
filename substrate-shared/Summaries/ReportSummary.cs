using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using substrate_shared.Descriptors.Enums;
using substrate_shared.interfaces.Reports;
using substrate_shared.Registries.enums;

namespace substrate_shared.Summaries
{
    public class ReportSummary : IReportSummary
    {
        private readonly List<string> _messages = new();
        private readonly Dictionary<DescriptorType, int> _counters = new();

        public void LogInfo(string message)
        {
            _messages.Add($"[INFO] {message}");
        }

        public void LogWarning(string message)
        {
            _messages.Add($"[WARN] {message}");
        }

        public void LogError(string message)
        {
            _messages.Add($"[ERROR] {message}");
        }

        public void LogException(Exception ex)
        {
            _messages.Add($"[EXCEPTION] {ex.GetType().Name}: {ex.Message}");
        }

        public void LogValidationError(IValidationError error)
        {
            _messages.Add($"[VALIDATION] {error}");
        }

        public IReadOnlyList<string> GetMessages()
        {
            return _messages.AsReadOnly();
        }

        /// <summary>
        /// Increment the counter for a given descriptor type.
        /// </summary>
        public void Increment(DescriptorType strategyType)
        {
            if (_counters.ContainsKey(strategyType))
            {
                _counters[strategyType]++;
            }
            else
            {
                _counters[strategyType] = 1;
            }

            // Optional: also log the increment
            _messages.Add($"[COUNT] {strategyType} incremented to {_counters[strategyType]}");
        }

        /// <summary>
        /// Retrieve current counts for all descriptor types.
        /// </summary>
        public IReadOnlyDictionary<DescriptorType, int> GetCounts()
        {
            return _counters;
        }

        private static readonly Regex IntentRegex = new Regex(
            @"IntentCommand:\s*(?<command>[^|]+)\s*\|\s*Narrative:\s*(?<narrative>[^|]+)\s*\|\s*Bias:\s*(?<bias>[^|]+)\s*\|\s*Group:\s*(?<group>[^|]+)\s*\|\s*Scale:\s*(?<scale>-?\d+)",
            RegexOptions.Compiled);

        public static TResult ParseIntentMessage<TResult>(string msg) where TResult : class
        {
            if (string.IsNullOrWhiteSpace(msg)) return null;

            var match = IntentRegex.Match(msg);
            if (!match.Success) return null;

            var parsed = new IntentMessage
            {
                Command = match.Groups["command"].Value.Trim(),
                Narrative = match.Groups["narrative"].Value.Trim(),
                Bias = match.Groups["bias"].Value.Trim(),
                Group = match.Groups["group"].Value.Trim(),
                Scale = int.TryParse(match.Groups["scale"].Value, out var s) ? s : 0,
                RawMessage = msg
            };

            return parsed as TResult;
        }
    }

    public class IntentMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Command { get; set; } = string.Empty;
        public string Narrative { get; set; } = string.Empty;

        // Original string values (parsed from raw message)
        public string Bias { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;

        // Strongly typed values
        public Bias BiasValue { get; set; } = substrate_shared.Registries.enums.Bias.Neutral;
        public NarrativeGroup GroupValue { get; set; } = NarrativeGroup.Equilibrium;
        public IntentAction IntentType { get; set; } = IntentAction.Observe;

        public int Scale { get; set; }
        public string RawMessage { get; set; } = string.Empty;
    }

}



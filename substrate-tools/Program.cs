using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using substrate_core.Startup;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Frames;
using substrate_shared.Descriptors.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Registries.enums;
using substrate_shared.Resolvers.Contract.Interfaces;
using substrate_shared.Summaries;

namespace substrate_tools
{
    class Program
    {
        static void Main(string[] args)
        {
            var report = new ReportSummary();

            // Initialize unified registry
            var boot = Startup.Initialize(
                typeof(BaseDescriptor).Assembly,
                typeof(IFrameResolver).Assembly
            );

            // Run through all IntentType values
            if (boot.Registry.TryGet<IntentDescriptor>(out var descriptor, out var resolver))
            {
                var intentDescriptor = (IntentDescriptor)descriptor;

                foreach (IntentAction action in Enum.GetValues(typeof(IntentAction)))
                {
                    intentDescriptor.IntentType = action;
                    var frame = new SimulationFrame { Intent = intentDescriptor };
                    resolver.Resolve(frame, report);
                }
            }

            // Define desired bias order explicitly: Positive, Neutral, Negative
            var biasOrder = new Dictionary<string, int>
            {
                ["Positive"] = 0,
                ["Neutral"]  = 1,
                ["Negative"] = 2
            };

            var intents = report.GetMessages()
                .Select(ReportSummary.ParseIntentMessage)
                .Where(x => x is not null)
                .Select(x => x!); // safe because of the Where above

            var groupedAndSorted = intents
                .GroupBy(x => x.Bias)
                .OrderBy(g => biasOrder.TryGetValue(g.Key, out var ord) ? ord : int.MaxValue)
                .SelectMany(g => g.OrderByDescending(x => x.Scale))
                .ToList();

            foreach (var entry in groupedAndSorted)
            {
                Console.WriteLine(entry.RawMessage);
            }

        }
    }
}

    public class IntentMessage
    {
        public string Command { get; set; } = string.Empty;
        public string Narrative { get; set; } = string.Empty;
        public string Bias { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public int Scale { get; set; }
        public string RawMessage { get; set; } = string.Empty;
    }

    public class ReportSummary : IReportSummary
    {
        private readonly List<string> _messages = new();

        // Regex to parse intent command log lines
        private static readonly Regex IntentRegex = new Regex(
            @"IntentCommand:\s*(?<command>[^|]+)\s*\|\s*Narrative:\s*(?<narrative>[^|]+)\s*\|\s*Bias:\s*(?<bias>[^|]+)\s*\|\s*Group:\s*(?<group>[^|]+)\s*\|\s*Scale:\s*(?<scale>-?\d+)",
            RegexOptions.Compiled);

        // Parser helper
        public static IntentMessage? ParseIntentMessage(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return null;

            var match = IntentRegex.Match(msg);
            if (!match.Success) return null;

            return new IntentMessage
            {
                Command    = match.Groups["command"].Value.Trim(),
                Narrative  = match.Groups["narrative"].Value.Trim(),
                Bias       = match.Groups["bias"].Value.Trim(),
                Group      = match.Groups["group"].Value.Trim(),
                Scale      = int.TryParse(match.Groups["scale"].Value, out var s) ? s : 0,
                RawMessage = msg
            };
        }

        // IReportSummary implementation
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
    }


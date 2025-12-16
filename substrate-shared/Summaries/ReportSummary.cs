using System;
using System.Collections.Generic;
using substrate_shared.interfaces.Reports;

namespace substrate_shared.Summaries
{
    public class ReportSummary : IReportSummary
    {
        private readonly List<string> _messages = new();

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
            _messages.Add($"[VALIDATION] {error.ToString()}");
        }

        public IReadOnlyList<string> GetMessages()
        {
            return _messages.AsReadOnly();
        }
    }
}
using System;
using System.Collections.Generic;
using substrate_shared.Descriptors.Enums;

namespace substrate_shared.interfaces.Reports
{
    public interface IReportSummary
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogException(Exception ex);
        void LogValidationError(IValidationError error);
        IReadOnlyList<string> GetMessages(); // Optional: for inspection/testing
        void Increment(DescriptorType strategyType);
    }
}
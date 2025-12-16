using System;
using substrate_shared.interfaces.Reports;

namespace substrate_shared.Errors
{
    public class ValidationError : Exception, IValidationError
    {
        public string Code { get; }
        public string Source { get; }

        public ValidationError(string message, string source = "Unknown", string code = "VALIDATION_ERROR")
            : base(message)
        {
            Code = code;
            Source = source;
        }

        public override string ToString() => $"[{Code}] {Source}: {Message}";
    }
}
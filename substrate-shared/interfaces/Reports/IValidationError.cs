namespace substrate_shared.interfaces.Reports
{
    public interface IValidationError : IPrismError
    {
        string Code { get; }
        string Source { get; }
        string Message { get; }

        string ToString(); // Optional override
    }
}
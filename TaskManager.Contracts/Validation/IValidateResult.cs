namespace TaskManager.Contracts.Validation
{
    public interface IValidateResult
    {
        bool IsValid { get; }

        string? FailureMessage { get; }
    }
}
namespace TaskManager.Contracts.Validation
{
    public class ValidateResult : IValidateResult
    {
        private ValidateResult(bool isValid, string? failureMessage = null)
        {
            IsValid = isValid;
            FailureMessage = failureMessage;
        }

        public bool IsValid { get; }

        public string? FailureMessage { get; }

        public static ValidateResult Valid() => new ValidateResult(true);

        public static ValidateResult Invalid(string message) => new ValidateResult(false, message);
    }
}

namespace TaskManager.Contracts.Validation
{
    public interface IValidated
    {
        IValidateResult Validate();
    }
}
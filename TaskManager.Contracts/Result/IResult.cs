namespace TaskManager.Contracts.Result
{
    public interface IResult<TData> : IResult
    {
        TData Data { get; }
    }

    public interface IResult
    {
        bool IsSuccess { get; }

        ErrorType? ErrorType { get; }

        string? FailureMessage { get; }
    }
}

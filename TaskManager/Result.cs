using System;
using System.Linq;
using TaskManager.Contracts.Result;

namespace TaskManager
{
    public class Result : IResult
    {
        protected Result(bool isSuccess, ErrorType? errorType = null, string? failureMessage = null, Exception? exception = null)
        {
            IsSuccess = isSuccess;
            ErrorType = errorType;

            var messages = new[]
            {
                failureMessage,
                exception?.Message,
                exception?.InnerException?.Message
            };
            FailureMessage = string.Join(" >> ", messages.Where(m => !string.IsNullOrWhiteSpace(m)));
        }

        public bool IsSuccess { get; }

        public string? FailureMessage { get; }

        public ErrorType? ErrorType { get; }

        public static Result Success()
            => new Result(true);

        public static Result Failure(ErrorType errorType, string? message = null, Exception? exception = null)
            => new Result(false, errorType, message, exception);
    }

    public class Result<TData> : Result, IResult<TData>
    {
        private Result(bool isSuccess, TData data, ErrorType? errorType = null, string? failureMessage = null, Exception? exception = null)
            : base(isSuccess, errorType, failureMessage, exception)
        {
            Data = data;
        }

        public TData Data { get; }

        public static Result<TData> Success(TData data)
            => new Result<TData>(true, data);

        public static Result<TData> Failure(TData data, ErrorType errorType, string? message = null, Exception? exception = null)
            => new Result<TData>(false, data, errorType, message, exception);

    }
}

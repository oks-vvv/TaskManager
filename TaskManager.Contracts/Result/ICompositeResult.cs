using System.Collections.Generic;

namespace TaskManager.Contracts.Result
{
    public interface ICompositeResult<TResult>
    {
        bool IsSuccess { get; }

        bool IsPartialFailure { get; }

        IEnumerable<IResult<TResult>> Results { get; }
    }
}

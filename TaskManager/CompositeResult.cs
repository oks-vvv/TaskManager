using System.Collections.Generic;
using System.Linq;
using TaskManager.Contracts.Result;

namespace TaskManager
{
    public class CompositeResult<TData> : ICompositeResult<TData>
    {
        public CompositeResult(IEnumerable<IResult<TData>> results)
        {
            IsSuccess = results.All(r => r.IsSuccess);
            IsPartialFailure = !IsSuccess && results.Any(r => r.IsSuccess);
            Results = results.ToList();
        }

        public bool IsSuccess { get; }

        public bool IsPartialFailure { get; }

        public IEnumerable<IResult<TData>> Results { get; }
    }
}

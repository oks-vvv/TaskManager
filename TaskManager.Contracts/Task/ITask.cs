using System;
using TaskManager.Contracts.Result;

namespace TaskManager.Contracts.Task
{
    public interface ITask : IDisposable
    {
        int Id { get; }

        DateTime CreatedAt { get; }

        Priority Priority { get; }

        IResult<TaskParams> Kill();
    }
}

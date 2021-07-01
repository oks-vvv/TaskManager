using System;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;

namespace TaskManager.Contracts
{
    public interface ITaskManager : IDisposable
    {
        IResult Add(TaskParams taskParams);

        IResult<TaskParams> Kill(int id);

        ICompositeResult<TaskParams> Kill(Priority priority);

        ICompositeResult<TaskParams> KillAll();
    }
}

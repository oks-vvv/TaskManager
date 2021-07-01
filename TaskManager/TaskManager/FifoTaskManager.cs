using System;
using System.Linq;
using TaskManager.Contracts;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;
using TaskManager.Process;

namespace TaskManager.TaskManager
{
    public class FifoTaskManager : TaskManager
    {
        private object @lock = new object();

        private readonly int capacity;

        public FifoTaskManager(IProcessService processService, ITaskStore taskStore, int capacity)
            : base(processService, taskStore)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            this.capacity = capacity;
        }

        public override IResult Add(TaskParams taskParams)
        {
            lock (@lock)
            {
                if (taskStore.Contains(taskParams.TaskId))
                {
                    return Result.Failure(ErrorType.AlreadyExists);
                }

                return !EnsuredCapacityAvailable()
                    ? Result.Failure(ErrorType.Failed, "Failed to ensure capacity")
                    : StartAndSaveTask(taskParams);
            }
        }

        private bool EnsuredCapacityAvailable()
        {
            if (taskStore.Count() < capacity)
            {
                return true;
            }

            var taskToKill = taskStore.Get()
                .OrderBy(t => t.CreatedAt)
                .First();
            var result = taskToKill.Kill();
            return result.IsSuccess;
        }
    }
}

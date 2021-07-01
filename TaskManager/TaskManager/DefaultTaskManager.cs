using System;
using TaskManager.Contracts;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;
using TaskManager.Process;

namespace TaskManager.TaskManager
{
    public class DefaultTaskManager : TaskManager
    {
        private object @lock = new object();

        private readonly int capacity;

        public DefaultTaskManager(IProcessService processService, ITaskStore taskStore, int capacity)
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

                if (taskStore.Count() >= capacity)
                {
                    return Result.Failure(ErrorType.CapacityLimit);
                }

                return StartAndSaveTask(taskParams);
            }
        }
    }
}

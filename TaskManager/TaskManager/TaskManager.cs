using System;
using System.Linq;
using TaskManager.Contracts;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;
using TaskManager.Process;

namespace TaskManager.TaskManager
{
    public abstract class TaskManager : ITaskManager
    {
        protected readonly IProcessService processService;
        protected readonly ITaskStore taskStore;

        private bool disposed = false;

        public TaskManager(IProcessService processService, ITaskStore taskStore)
        {
            this.processService = processService;
            this.taskStore = taskStore;
        }

        public abstract IResult Add(TaskParams taskParams);

        public IResult<TaskParams> Kill(int id)
        {
            var task = taskStore.Get(id);

            return task == null
                ? Result<TaskParams>.Failure(new TaskParams(id, Priority.Uknown), ErrorType.NotFound)
                : task.Kill();
        }

        public ICompositeResult<TaskParams> Kill(Priority priority)
        {
            var tasksToKill = taskStore.Get()
                .Where(t => t.Priority == priority)
                .ToList();

            var results = tasksToKill.Select(t => t.Kill());

            return new CompositeResult<TaskParams>(results);
        }

        public ICompositeResult<TaskParams> KillAll()
        {
            var tasksToKill = taskStore.Get().ToList();

            var results = tasksToKill.Select(t => t.Kill());

            return new CompositeResult<TaskParams>(results);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected IResult StartAndSaveTask(TaskParams taskParams)
        {
            try
            {
                var processId = processService.Start(taskParams.Priority);

                var task = new TaskInstance(taskParams, processId, processService, taskStore);
                taskStore.Add(task);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ErrorType.Failed, "Failed to start and save task", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                var tasks = taskStore?.Get().ToList() ?? Enumerable.Empty<ITask>();
                foreach(var task in tasks)
                {
                    task.Dispose();
                }
            }

            disposed = true;
        }
    }
}

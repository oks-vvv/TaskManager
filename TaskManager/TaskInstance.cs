using System;
using TaskManager.Contracts;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;
using TaskManager.Process;

namespace TaskManager
{
    public class TaskInstance : ITask
    {
        private object @lock = new object();

        private readonly int processId;
        private readonly IProcessService processService;
        private readonly ITaskStore taskStore;

        private bool disposed = false;

        public TaskInstance(TaskParams taskParams, int processId, IProcessService processService, ITaskStore taskStore)
        {
            this.Id = taskParams.TaskId;
            this.Priority = taskParams.Priority;
            this.CreatedAt = DateTime.UtcNow;

            this.processId = processId;
            this.processService = processService;
            this.taskStore = taskStore;
        }

        public int Id { get; }

        public Priority Priority { get; }

        public DateTime CreatedAt { get; }

        public IResult<TaskParams> Kill()
        {
            var taskParams = new TaskParams(Id, Priority);

            lock (@lock)
            {
                if (!taskStore.Contains(Id))
                {
                    return Result<TaskParams>.Failure(taskParams, ErrorType.NotFound);
                }

                try
                {
                    processService.Kill(processId);

                    return !taskStore.Remove(Id)
                        ? Result<TaskParams>.Failure(taskParams, ErrorType.Failed, "Failed to remove")
                        : Result<TaskParams>.Success(taskParams);
                }
                catch (Exception ex)
                {
                    return Result<TaskParams>.Failure(taskParams, ErrorType.Failed, "Failed to remove", ex);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                processService?.Kill(processId);
            }

            disposed = true;
        }
    }
}

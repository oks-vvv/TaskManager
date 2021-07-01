using System;
using TaskManager.Contracts.Task;

namespace TaskManager.Api.Models
{
    public record TaskInformation : TaskRequest
    {
        public TaskInformation(ITask task)
        {
            Id = task.Id;
            Priority = task.Priority;
            CreatedAt = task.CreatedAt;
        }

        public DateTime CreatedAt { get; init; }
    }
}

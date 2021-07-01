namespace TaskManager.Contracts.Task
{
    public record TaskParams
    {
        public TaskParams(int taskId, Priority priority)
        {
            TaskId = taskId;
            Priority = priority;
        }

        public int TaskId { get; }

        public Priority Priority { get; }
    }
}

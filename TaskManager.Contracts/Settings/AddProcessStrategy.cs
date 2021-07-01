namespace TaskManager.Settings
{
    public enum AddProcessStrategy
    {
        /// <summary>
        /// We can accept new processes till when there is capacity inside the Task Manager,
        /// otherwise we won’t accept any new process
        /// </summary>
        Default = 0,

        /// <summary>
        /// Accept all new processes, killing and removing from the TM list the oldest one (First-In, First-Out)
        /// when the max size is reached
        /// </summary>
        FifoAdd = 1,

        /// <summary>
        /// When the max size is reached, should result into an evaluation:
        /// if the new process has a higher priority compared to any of the existing one,
        /// we remove the lowest priority that is the oldest, otherwise we skip it
        /// </summary>
        PriorityFifoAdd = 2,
    }
}
using TaskManager.Contracts.Task;

namespace TaskManager.Process
{
    public interface IProcessService
    {
        int Start(Priority priority);
        void Kill(int processId);
    }
}

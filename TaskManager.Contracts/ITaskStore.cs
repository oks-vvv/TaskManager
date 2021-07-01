using System.Collections.Generic;
using TaskManager.Contracts.Task;

namespace TaskManager.Contracts
{
    public interface ITaskStore
    {
        IEnumerable<ITask> Get();

        bool Contains(int taskId);

        ITask? Get(int taskId);

        void Add(ITask task);

        bool Remove(int taskId);

        int Count();
    }
}

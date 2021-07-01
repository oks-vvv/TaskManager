using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Contracts;
using TaskManager.Contracts.Task;

namespace TaskManager
{
    public class InMemoryTaskStore : ITaskStore
    {
        private ConcurrentDictionary<int, ITask> tasks;

        public InMemoryTaskStore() => tasks = new ConcurrentDictionary<int, ITask>();

        public void Add(ITask task)
        {
            if (!tasks.TryAdd(task.Id, task))
            {
                throw new InvalidOperationException($"Task with id {task.Id} already exists");
            }
        }

        public bool Contains(int taskId) => tasks.ContainsKey(taskId);

        public int Count() => tasks.Count;

        public IEnumerable<ITask> Get() => tasks.Values.ToList();

        public ITask? Get(int taskId) => tasks.GetValueOrDefault(taskId);

        public bool Remove(int taskId) => tasks.TryRemove(taskId, out _);
    }
}

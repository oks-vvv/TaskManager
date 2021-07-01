using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Contracts;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskStore taskStore;
        private readonly ITaskManager taskManager;

        public TasksController(ITaskStore taskStore, ITaskManager taskManager)
        {
            this.taskStore = taskStore;
            this.taskManager = taskManager;
        }

        [HttpGet]
        public IEnumerable<TaskInformation> List(TaskSortBy sortBy)
        {
            var tasks = taskStore.Get();
            var ordered = sortBy switch
            {
                TaskSortBy.DateCreatedAt => tasks.OrderBy(t => t.CreatedAt),
                TaskSortBy.Priority => tasks.OrderBy(t => t.Priority),
                TaskSortBy.Id => tasks.OrderBy(t => t.Id),
                _ => throw new NotSupportedException($"{sortBy} is not supported"),
            };

            return ordered.Select(t => new TaskInformation(t));
        }

        [HttpDelete("{priority}")]
        public ActionResult Kill(Priority priority)
        {
            var result = taskManager.Kill(priority);

            return HandleCompositeResult(result);
        }

        [HttpDelete("all")]
        public ActionResult KillAll()
        {
            var result = taskManager.KillAll();

            return HandleCompositeResult(result);
        }

        private ActionResult HandleCompositeResult(ICompositeResult<TaskParams> result)
        {
            var data = result.Results.Select(r => r.Data).ToList();

            return (result.IsSuccess, result.IsPartialFailure) switch
            {
                (true, _) => Ok(data),
                (false, true) => StatusCode(StatusCodes.Status207MultiStatus, result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}

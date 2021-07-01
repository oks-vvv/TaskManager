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
    public class TaskController : ControllerBase
    {
        private readonly ITaskManager taskManager;

        public TaskController(ITaskManager taskManager)
        {
            this.taskManager = taskManager;
        }

        [HttpPost()]
        public ActionResult Add([FromBody]TaskRequest task)
        {
            var result = taskManager.Add(new TaskParams(task.Id, task.Priority));

            return (result.IsSuccess, result.ErrorType) switch
            {
                (true, _) => Accepted(),
                (false, ErrorType.AlreadyExists) => Conflict(result),
                (false, ErrorType.CapacityLimit) => StatusCode(StatusCodes.Status429TooManyRequests, result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpDelete("{id}")]
        public ActionResult Kill(int id)
        {
            var result = taskManager.Kill(id);

            return (result.IsSuccess, result.ErrorType) switch
            {
                (true, _) => Ok(result.Data),
                (false, ErrorType.NotFound) => NotFound(result.Data),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}

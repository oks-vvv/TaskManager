using System.ComponentModel.DataAnnotations;
using TaskManager.Contracts.Task;

namespace TaskManager.Api.Models
{
    public record TaskRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id of a task should be positive number")]
        public int Id { get; init; }

        [Required]
        public Priority Priority { get; init; }
    }
}

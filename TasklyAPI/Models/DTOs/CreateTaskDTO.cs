using System.ComponentModel.DataAnnotations;
using TasklyAPI.Enums;

namespace TasklyAPI.Models.DTOs
{
	public class CreateTaskDTO
	{
		[Required]
		[MaxLength(500)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(1500)]
		public string? Description { get; set; } = string.Empty;

		[Required]
		public TaskPriority Priority { get; set; }

		public DateTime? Deadline { get; set; }

		public bool IsFinished { get; set; } = false;
	}
}

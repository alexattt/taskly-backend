using System.ComponentModel.DataAnnotations;
using TasklyAPI.Enums;

namespace TasklyAPI.Models.DTOs
{
	public class TaskDTO
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(500)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(1500)]
		public string? Description { get; set; } = string.Empty;

		[Required]
		public TaskPriority Priority { get; set; }

		[Required]
		public DateTime? Deadline { get; set; }

		public bool IsFinished { get; set; } = false;

		public DateTime CreatedAt { get; set; }
	}
}

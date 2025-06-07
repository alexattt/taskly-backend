using System.ComponentModel.DataAnnotations;
using TasklyAPI.Enums;

namespace TasklyAPI.Models
{
	public class TaskItem
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(500)]
		public string Name { get; set; }

		[MaxLength(1500)]
		public string? Description { get; set; }

		[Required]
		public TaskPriority Priority { get; set; }

		public DateTime? Deadline { get; set; }

		public bool IsFinished { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public string UserId { get; set; }

		public User? User { get; set; }
	}
}

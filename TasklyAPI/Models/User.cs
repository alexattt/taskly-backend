using Microsoft.AspNetCore.Identity;

namespace TasklyAPI.Models
{
	public class User : IdentityUser
	{
		public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
	}
}

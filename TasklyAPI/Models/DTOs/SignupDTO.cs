using System.ComponentModel.DataAnnotations;

namespace TasklyAPI.Models.DTOs
{
	public class SignupDTO
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[MinLength(6)]
		public string Password { get; set; } = string.Empty;

		[Required]
		[Compare("Password")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}

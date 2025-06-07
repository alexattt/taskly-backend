using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TasklyAPI.Models.DTOs;
using TasklyAPI.Services;

namespace TasklyAPI.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly AuthService _authService;

		public AuthController(AuthService authService)
		{
			_authService = authService;
		}

		/// <summary>
		/// Creates an account for new user
		/// </summary>
		/// <param name="signupDto">SignupDTO object</param>
		[SwaggerResponse(200, "New user created successfully")]
		[SwaggerResponse(400, "SignupDTO validation or user signup failed")]
		[HttpPost("register")]
		public async Task<IActionResult> SignupUser([FromBody] SignupDTO signupDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var result = await _authService.SignupAsync(signupDto);

			if (!result)
			{
				return BadRequest("User sign up failed");
			}

			return Ok();
		}

		/// <summary>
		/// Logs user into the system
		/// </summary>
		/// <param name="loginDto">LoginDTO object</param>
		/// <returns>JWT token</returns>
		[SwaggerResponse(200, "User login successful", typeof(string))]
		[SwaggerResponse(400, "LoginDTO validation failed")]
		[SwaggerResponse(401, "Invalid login credentials")]
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var token = await _authService.LoginAsync(loginDto);
			if (token == null)
			{
				return Unauthorized("Invalid credentials");
			}

			return Ok(new { jwtToken = token });
		}
	}
}

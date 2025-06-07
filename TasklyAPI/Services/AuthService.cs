using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TasklyAPI.Models;
using TasklyAPI.Models.DTOs;

namespace TasklyAPI.Services
{
	public class AuthService
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _configuration;

		public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
		}

		public async Task<string?> LoginAsync(LoginDTO loginDto)
		{
			var user = await _userManager.FindByEmailAsync(loginDto.Email);

			if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
			{
				return null;
			}

			return GenerateJwtToken(user);
		}

		public async Task<bool> SignupAsync(SignupDTO signupDto)
		{
			var user = new User
			{
				UserName = signupDto.Email,
				Email = signupDto.Email
			};

			var result = await _userManager.CreateAsync(user, signupDto.Password);
			return result.Succeeded;
		}

		private string GenerateJwtToken(User user)
		{
			var jwtSettings = _configuration.GetSection("JWT");
			var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(24),
				signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using TasklyAPI.Models;
using TasklyAPI.Models.DTOs;
using TasklyAPI.Services;

namespace TasklyTests.ServiceTests
{
	public class AuthServiceTests
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _configuration;
		private readonly AuthService _authService;

		public AuthServiceTests()
		{
			_userManager = Substitute.For<UserManager<User>>(
				Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

			_signInManager = Substitute.For<SignInManager<User>>(
				_userManager,
				Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
				Substitute.For<IUserClaimsPrincipalFactory<User>>(),
				null, null, null, null);

			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["JWT:Secret"] = "ThisIsAVeryLongSecretKeyForJwtTokenGeneration123456789",
				["JWT:Issuer"] = "TestIssuer",
				["JWT:Audience"] = "TestAudience"
			});
			_configuration = configurationBuilder.Build();

			_authService = new AuthService(_userManager, _signInManager, _configuration);
		}

		[Fact]
		public async Task SignupAsync_ShouldReturnTrue_WhenRegistrationSucceeds()
		{
			// Arrange
			var signupDto = new SignupDTO
			{
				Email = "test@example.com",
				Password = "Test123!",
				ConfirmPassword = "Test123!"
			};

			_userManager.CreateAsync(Arg.Any<User>(), signupDto.Password)
				.Returns(IdentityResult.Success);

			// Act
			var result = await _authService.SignupAsync(signupDto);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task SignupAsync_ShouldReturnFalse_WhenRegistrationFails()
		{
			// Arrange
			var registerDto = new SignupDTO
			{
				Email = "test@example.com",
				Password = "weak"
			};

			_userManager.CreateAsync(Arg.Any<User>(), registerDto.Password)
				.Returns(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

			// Act
			var result = await _authService.SignupAsync(registerDto);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsValid()
		{
			// Arrange
			var loginDto = new LoginDTO
			{
				Email = "test@example.com",
				Password = "Test123!"
			};

			var user = new User
			{
				Id = "test-id",
				Email = loginDto.Email
			};

			_userManager.FindByEmailAsync(loginDto.Email).Returns(user);
			_userManager.CheckPasswordAsync(user, loginDto.Password).Returns(true);

			// Act
			var result = await _authService.LoginAsync(loginDto);

			// Assert
			Assert.NotNull(result);
			Assert.NotEmpty(result);
		}

		[Fact]
		public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
		{
			// Arrange
			var loginDto = new LoginDTO
			{
				Email = "test_fake@example.com",
				Password = "Test123!"
			};

			_userManager.FindByEmailAsync(loginDto.Email).Returns((User?)null);

			// Act
			var result = await _authService.LoginAsync(loginDto);

			// Assert
			Assert.Null(result);
		}
	}
}

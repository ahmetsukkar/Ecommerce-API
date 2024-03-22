using Ecom.API.Errors;
using Ecom.API.Extensions;
using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Policy;

namespace Ecom.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AccountsController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenServices _tokenServices;

		public AccountsController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, ITokenServices tokenServices)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_tokenServices = tokenServices;
		}

		[HttpGet("get-all-user")]
		[AllowAnonymous]
		public ActionResult GetAllUsers()
		{
			var users = _userManager.Users.ToList();

			var result = users.Select(u => new User()
			{
				Id = u.Id,
				DisplayName = u.DisplayName,
				Email = u.Email
			}).ToList();

			return Ok(result);
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<ActionResult> Login(LoginDto loginDto)
		{
			// Find the user by email
			var user = await _userManager.FindByEmailAsync(loginDto.Email);

			// Check if the user does not exist
			if (user == null) return Unauthorized(new BaseCommonResponse(401, $"this email {loginDto.Email} is not found"));

			// Check if the provided password is correct for the user
			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
			if (result == null || result.Succeeded == false) return Unauthorized(new BaseCommonResponse(401, $"check your password, it's not correct"));

			// Retrieve the userRoles
			var userRoles = await _userManager.GetRolesAsync(user);

			// Mapping to UserDto
			var userDto = new UserDto
			{
				Id = user.Id,
				DisplayName = user.DisplayName,
				Email = user.Email,
				Roles = string.Join(", ", userRoles.Select(r => r).ToList()),
				Token = _tokenServices.CreateToke(user)
			};

			// Return an OK response with the user details and token
			return Ok(userDto);
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<ActionResult> Register(RegisterDto registerDto)
		{
			// Check if the userEmail does not exist
			var isEmailExist = await CheckEmailExist(registerDto.Email);
			if (isEmailExist == true)
				return new BadRequestObjectResult(new ApiValidationErrorResponse
				{
					Errors = new[] { "This Email is already token" }
				});

			// Check if the Role does not exist
			var roleResult = await _roleManager.RoleExistsAsync(registerDto.Role);
			if (roleResult == false)
				return new BadRequestObjectResult(new ApiValidationErrorResponse
				{
					Errors = new[] { "This Role is not exist" }
				});

			// Create new user
			var user = new AppUser
			{
				DisplayName = registerDto.DisplayName,
				UserName = registerDto.Email,
				Email = registerDto.Email
			};

			// Add the new product
			var userResult = await _userManager.CreateAsync(user, registerDto.Password);

			// if something happen while create new user
			if (userResult.Succeeded == false)
				return new BadRequestObjectResult(new ApiValidationErrorResponse
				{
					Errors = userResult.Errors.Select(f => f.Description).ToList()
				});

			// Add User to Role
			await _userManager.AddToRoleAsync(user, registerDto.Role);

			// Mapping to UserDto
			var userDto = new UserDto
			{
				Id = user.Id,
				DisplayName = registerDto.DisplayName,
				Email = registerDto.Email,
				Roles = registerDto.Role,
				Token = _tokenServices.CreateToke(user)
			};

			// Return an OK response with the user details and token
			return Ok(userDto);
		}

		[HttpGet("get-current-user-info")]
		public async Task<ActionResult> GetCurrentUserInfo()
		{
			var user = await _userManager.FindEmailByClaimPrincipleAsync(HttpContext.User);
			return Ok(new UserDto
			{
				Id = user.Id,
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = _tokenServices.CreateToke(user)
			});
		}

		[HttpGet("check-email-exist/{email}")]
		public async Task<bool> CheckEmailExist([FromRoute] string email)
		{
			return await _userManager.FindByEmailAsync(email) != null;
		}

	}
}

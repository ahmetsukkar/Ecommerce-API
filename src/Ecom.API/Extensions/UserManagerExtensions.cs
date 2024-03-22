using Ecom.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecom.API.Extensions
{
	public static class UserManagerExtensions
	{
		public static async Task<AppUser> FindEmailByClaimPrincipleAsync(this UserManager<AppUser> userManager,
			ClaimsPrincipal user)
		{
			var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
			return await userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
		}
	}
}

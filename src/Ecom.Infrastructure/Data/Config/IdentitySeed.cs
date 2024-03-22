using Ecom.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Data.Config
{
	public class IdentitySeed
	{

		public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
		{
			if (!roleManager.Roles.Any())
			{
				// Define basic roles
				var roles = new[] { "Admin", "Employee", "Customer" };

                foreach (var role in roles)
                {
					await roleManager.CreateAsync(new IdentityRole(role));
				}
            }
		}

		public static async Task SeeUserAsync(UserManager<AppUser> userManager)
		{
			if (!userManager.Users.Any())
			{
				// Create a new user (Default Admin User)
				var user = new AppUser
				{
					DisplayName = "admin",
					Email = "admin@admin.com",
					UserName = "admin@admin.com"
				};

				await userManager.CreateAsync(user, "P@ssw0rd");

				// Assign the Admin role to the user
				await userManager.AddToRoleAsync(user, "Admin");
			}
		}
	}
}

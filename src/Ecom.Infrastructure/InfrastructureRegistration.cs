using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Data.Config;
using Ecom.Infrastructure.Repositories;
using Ecom.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure
{
    public static class InfrastructureRegistration
	{
		public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			//configure Token Services
			services.AddScoped<ITokenServices, TokenServices>();
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<IUnitOfWork, UnitOfWork>(); //insted of register services one by one (Create UnitOfWork Class to collect all Repository classes under One Unit)

			//Configure DB
			services.AddDbContext<ApplicationDbContext>(opt =>
			{
				opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});

			//Configure Identity
			services.AddIdentity<AppUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			services.AddMemoryCache();
			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
					.AddJwtBearer(opt =>
					{
						opt.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
							ValidIssuer = configuration["Token:Issuer"],
							ValidateIssuer = true,
							ValidateAudience = false
						};
					});

			return services;
		}

		public static async void InfrastructreConfigurationMiddleware(this IApplicationBuilder app)
		{
			// To Create the Database Tabels __ Depending on ConnectionString that exist in appsettings.json file
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				db.Database.Migrate();
			}

			using (var scope = app.ApplicationServices.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

				await IdentitySeed.SeedRoleAsync(roleManager);
				await IdentitySeed.SeeUserAsync(userManager);
			}
		}
	}
}

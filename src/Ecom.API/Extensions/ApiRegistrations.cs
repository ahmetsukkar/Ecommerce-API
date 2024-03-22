using Ecom.API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Extensions
{
	// Extension class for registering API behaviors
	public static class ApiRegistrations
	{
		public static IServiceCollection AddAPIRegistration(this IServiceCollection services)
		{
			services.Configure<ApiBehaviorOptions>(opt =>
			{
				opt.InvalidModelStateResponseFactory = context =>
				{
					// validation error response containing model state errors
					var errorResponse = new ApiValidationErrorResponse
					{
						Errors = context.ModelState
								.Where(x => x.Value.Errors.Count > 0)
								.SelectMany(x => x.Value.Errors)
								.Select(x => x.ErrorMessage).ToArray()
					};
					return new BadRequestObjectResult(errorResponse);
				};
			});

			return services;
		}
	}
}

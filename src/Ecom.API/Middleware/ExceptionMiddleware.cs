using Ecom.API.Errors;
using System.Net;
using System.Text.Json;

namespace Ecom.API.Middleware
{
	// Middleware to handle exceptions globally
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext httpcontext)
		{
			try
			{
				await _next(httpcontext);
				_logger.LogInformation("Success");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"This Error come from Exception Middleware {ex.Message}");

				httpcontext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				httpcontext.Response.ContentType = "application/json";
				var response = new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString());

				var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
				var jsonResponse = JsonSerializer.Serialize(response, options);
				await httpcontext.Response.WriteAsync(jsonResponse);
			}
		}
	}
}

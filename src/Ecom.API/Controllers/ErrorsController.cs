using Ecom.API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
	// Controller to handle errors and return a common response
	[Route("errors/")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)] // Hide this API from the Swagger documentation
	public class ErrorsController : ControllerBase
	{
		[HttpGet("{statusCode}")]
		public ActionResult Errors(int statusCode)
		{
			return new ObjectResult(new BaseCommonResponse(statusCode));
		}
	}
}

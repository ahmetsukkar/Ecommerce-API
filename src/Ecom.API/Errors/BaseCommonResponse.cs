namespace Ecom.API.Errors
{
	public class BaseCommonResponse
	{
		public int StatusCode { get; set; }
		public string Message { get; set; }

		public BaseCommonResponse(int statusCode, string message = null)
		{
			StatusCode = statusCode;
			Message = message ?? GetDefaultMesaageForStatusCode(statusCode);
		}

		private string GetDefaultMesaageForStatusCode(int statusCode)
		{
			switch (statusCode)
			{
				case 204:
					return "no content";
				case 400:
					return "bad request"; 
				case 401:
					return "not authorize";
				case 404:
					return "resource not found";
				case 500:
					return "server error";
				default:
					return null;
			}
		}
	}
}

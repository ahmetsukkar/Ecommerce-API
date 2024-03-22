namespace Ecom.API.Errors
{
	public class ApiException : BaseCommonResponse
	{
        public string Details { get; set; }
        public ApiException(int statusCode, string message = null, string details = null) : base(statusCode, message)
		{
			Details = details;
		}
	}
}

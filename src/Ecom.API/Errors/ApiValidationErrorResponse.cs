namespace Ecom.API.Errors
{
	public class ApiValidationErrorResponse : BaseCommonResponse
	{
		public ApiValidationErrorResponse() : base(400) // The error code for this status is 400 for all
		{
		}

		public IEnumerable<string> Errors { get; set; }
	}
}

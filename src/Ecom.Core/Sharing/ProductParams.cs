namespace Ecom.Core.Sharing
{
	public class ProductParams
	{
		private int MaxPageSize { get; set; } = 20;

		private int _pageSize { get; set; } = 10;

		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
		}

		public int PageNumber { get; set; } = 1;

		public string Sort { get; set; }

		public int? CategoryId { get; set; }

		private string _search;

		public string Search
		{
			get { return _search; }
			set { _search = !string.IsNullOrEmpty(value) ? value.ToLower() : null; }
		}

	}
}

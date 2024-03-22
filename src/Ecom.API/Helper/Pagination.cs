namespace Ecom.API.Helper
{
	public class Pagination<T> where T : class
	{
		public Pagination(int pageNumber, int pageSize, int totalCount, List<T> data)
		{
			PageNumber = pageNumber;
			PageSize = pageSize;
			TotalCount = totalCount;
			Data = data;
		}

		public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
		public List<T> Data { get; set; }
	}
}

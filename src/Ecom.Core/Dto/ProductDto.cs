using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecom.Core.Dto
{
	public class ProductDto
	{
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[Required]
		[Range(1, 999, ErrorMessage = "Price Limited By {0} and {1}")]
		public decimal Price { get; set; }
	}

	public class ResponseProductDto : ProductDto
	{
		public int Id { get; set; }
		public string CategorName { get; set; }
		public string ProductPicture { get; set; }
	}

	public class CreateProductDto : ProductDto
	{
		public int CategoryId { get; set; } = 1; //Default CategoryId, Books Category as default category for Now
		public IFormFile Image { get; set; }
	}

	public class UpdateProductDto : ProductDto
	{
		public int Id { get; set; }
		public int CategoryId { get; set; } = 1;
		public string OldImage { get; set; }
		public IFormFile Image { get; set; }
	}

}

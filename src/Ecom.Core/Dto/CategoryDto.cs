using System.ComponentModel.DataAnnotations;

namespace Ecom.Core.Dto
{
	public class CategoryDto
	{
		[Required]
		public string Name { get; set; }
		public string Discription { get; set; }
	}

	public class ResponseCategoryDto : CategoryDto
	{
		public int Id { get; set; }
	}

	public class UpdateCategoryDto : CategoryDto
	{
		public int Id { get; set; }
	}
}

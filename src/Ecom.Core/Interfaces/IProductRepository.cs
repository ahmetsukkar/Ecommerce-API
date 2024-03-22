using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
	public interface IProductRepository : IGenericRepository<Product>
	{
		Task<IEnumerable<Product>> GetAllAsync(ProductParams productParams);

		Task<Product> AddAsync(CreateProductDto productDto);

		Task<Product> UpdateAsync(int id, UpdateProductDto productDto);

		Task<bool> DeleteWithPictureAsync(int id);
	}
}

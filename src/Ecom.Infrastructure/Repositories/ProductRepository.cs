using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Sharing;
using Ecom.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
	public class ProductRepository : GenericRepository<Product>, IProductRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IFileProvider _fileProvider;

		public ProductRepository(ApplicationDbContext context, IFileProvider fileProvider) : base(context)
		{
			_context = context;
			_fileProvider = fileProvider;
		}

		public async Task<IEnumerable<Product>> GetAllAsync(ProductParams productParams)
		{
			var productsQuery = await _context.Products
									  .Include(i => i.Category)
									  .AsNoTracking().ToListAsync();

			//search by name
			if (!string.IsNullOrEmpty(productParams.Search))
			{
				productsQuery = productsQuery.Where(f => f.Name.ToLower().Contains(productParams.Search)).ToList();
			}

			//filtering by categoryId
			if (productParams.CategoryId.HasValue)
				productsQuery = productsQuery.Where(f => f.CategoryId == productParams.CategoryId.Value).ToList();

			//sorting
			productsQuery = productParams.Sort switch
			{
				"PriceAsce" => productsQuery.OrderBy(f => f.Price).ToList(),
				"PriceDesc" => productsQuery.OrderByDescending(f => f.Price).ToList(),
				_ => productsQuery.OrderBy(f => f.Name).ToList(),
			};

			//paging
			productsQuery = productsQuery.Skip(productParams.PageSize * (productParams.PageNumber - 1)).Take(productParams.PageSize).ToList();

			return productsQuery;
		}

		public async Task<Product> AddAsync(CreateProductDto productDto)
		{
			string newProductPicturePath = await UploadProductImageAsync(productDto.Image);

			var newProduct = new Product()
			{
				Name = productDto.Name,
				Description = productDto.Description,
				Price = productDto.Price,
				ProductPicture = newProductPicturePath,
				CategoryId = productDto.CategoryId
			};

			await _context.AddAsync(newProduct);
			await _context.SaveChangesAsync();
			return newProduct;
		}

		public async Task<Product> UpdateAsync(int id, UpdateProductDto updateProductDto)
		{
			var currentProduct = await _context.Products.FindAsync(id);
			if (currentProduct == null) return null;

			string newProductPicturePath = await UploadProductImageAsync(updateProductDto.Image);

			//remove old picture
			DeleteOldProductImageAsync(currentProduct.ProductPicture);

			currentProduct.Name = updateProductDto.Name;
			currentProduct.Description = updateProductDto.Description;
			currentProduct.Price = updateProductDto.Price;
			currentProduct.CategoryId = updateProductDto.CategoryId;
			currentProduct.ProductPicture = newProductPicturePath;

			// Mapping To Product Type to return
			var product = new Product()
			{
				Id = currentProduct.Id,
				Name = updateProductDto.Name,
				Description = updateProductDto.Description,
				Price = updateProductDto.Price,
				CategoryId = updateProductDto.CategoryId,
				ProductPicture = newProductPicturePath
			};

			_context.Products.Update(currentProduct);
			await _context.SaveChangesAsync();
			return product;
		}

		public async Task<bool> DeleteWithPictureAsync(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product != null)
			{
				_context.Products.Remove(product);
				_context.SaveChanges();

				//remove picture
				DeleteOldProductImageAsync(product.ProductPicture);

				return true;
			}

			return false;
		}

		#region "Private Helping Functions"

		private async Task<string> UploadProductImageAsync(IFormFile image)
		{
			if (image != null)
			{
				var productDirectory = "/images/products/";
				var productName = String.Format("{0}-{1}", Guid.NewGuid(), image.FileName);
				if (!Directory.Exists("wwwroot" + productDirectory))
				{
					Directory.CreateDirectory("wwwroot" + productDirectory);
				}
				var src = productDirectory + productName;
				var picInfo = _fileProvider.GetFileInfo(src);
				var rootPath = picInfo.PhysicalPath;
				using (var fileStream = new FileStream(rootPath, FileMode.Create))
				{
					await image.CopyToAsync(fileStream);
				}

				return src;
			}

			return string.Empty;
		}

		private void DeleteOldProductImageAsync(string currentProductPicture)
		{
			if (!string.IsNullOrEmpty(currentProductPicture))
			{
				var pictureInfo = _fileProvider.GetFileInfo(currentProductPicture);
				var rootpath = pictureInfo.PhysicalPath;
				System.IO.File.Delete(rootpath);
			}
		}

		#endregion
	}
}

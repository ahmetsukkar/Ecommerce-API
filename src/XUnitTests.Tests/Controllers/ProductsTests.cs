using Ecom.API.Controllers;
using Ecom.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Mvc;
using Ecom.API.Helper;
using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.API.Errors;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace XUnitTests.Tests.Controllers
{
	public class ProductsTests
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IConfiguration> _mockConfig;
		private readonly ProductsController _controller;

		public ProductsTests()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockConfig = new Mock<IConfiguration>();
			_controller = new ProductsController(_mockUnitOfWork.Object, _mockConfig.Object);
		}

		[Fact]
		[Trait("Category", "Product_GetServices")]
		public async Task GetAllProducts_ReturnsOkObjectResult_WithValidPagination()
		{
			// Arrange
			var productParam = new ProductParams { PageNumber = 1, PageSize = 10 };
			var mockProducts = new List<Product> { /* Mock products */ };
			var totalCount = 20; // Total count of products

			_mockUnitOfWork.Setup(u => u.ProductRepository.GetAllAsync(productParam)).ReturnsAsync(mockProducts);
			_mockUnitOfWork.Setup(u => u.ProductRepository.CountAsync()).ReturnsAsync(totalCount);

			// Act
			var result = await _controller.GetAllProducts(productParam);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var pagination = Assert.IsType<Pagination<ResponseProductDto>>(okResult.Value);
			Assert.Equal(productParam.PageNumber, pagination.PageNumber);
			Assert.Equal(productParam.PageSize, pagination.PageSize);
			Assert.Equal(totalCount, pagination.TotalCount);
			Assert.Equal(mockProducts.Count, pagination.Data.Count());
		}

		[Fact]
		[Trait("Category", "Product_GetServices")]
		public async Task GetAllProducts_ReturnsBadRequest_WhenInvalidPageNumberProvided()
		{
			// Arrange
			var productParam = new ProductParams { PageNumber = -1, PageSize = 10 }; // Invalid page number

			// Act
			var result = await _controller.GetAllProducts(productParam);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			var response = Assert.IsType<BaseCommonResponse>(badRequestResult.Value);
			Assert.Equal(400, response.StatusCode);
			Assert.Equal("Invalid parameters", response.Message);
		}

		[Fact]
		[Trait("Category", "Product_GetServices")]
		public async Task GetProductById_ReturnsOkObjectResult_WhenProductExists()
		{
			// Arrange
			var productId = 1;
			var mockProduct = new Product
			{
				Id = productId,
				Name = "Test Product",
				Description = "Test Description",
				Price = 10.99m,
				Category = new Category { Id = 1, Name = "Test Category" }
			};

			_mockUnitOfWork.Setup(u => u.ProductRepository.GetByIdAsync(productId, x => x.Category))
						   .ReturnsAsync(mockProduct);

			// Act
			var result = await _controller.GetProductById(productId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var productDto = Assert.IsType<ResponseProductDto>(okResult.Value);
			Assert.Equal(productId, productDto.Id);
		}

		[Fact]
		[Trait("Category", "Product_GetServices")]
		public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
		{
			// Arrange
			var productId = 1;
			Product mockProduct = null; // Simulate product not found

			_mockUnitOfWork.Setup(u => u.ProductRepository.GetByIdAsync(productId, x => x.Category))
						   .ReturnsAsync(mockProduct);

			// Act
			var result = await _controller.GetProductById(productId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			var response = Assert.IsType<BaseCommonResponse>(notFoundResult.Value);
			Assert.Equal(404, response.StatusCode);
		}

		[Fact]
		[Trait("Category", "Product_AddServices")]
		public async Task AddNewProduct_ReturnsOkObjectResult_WhenCategoryExistsAndProductAddedSuccessfully()
		{
			// Arrange
			var createProductDto = new CreateProductDto
			{
				Name = "Test Product",
				Description = "Test Description",
				Price = 10.99m,
				CategoryId = 1, // Valid category ID
			};
			var mockCategory = new Category { Id = 1, Name = "Test Category" };
			var mockProduct = new Product { Id = 1, Name = "Test Product", Category = mockCategory };

			_mockUnitOfWork.Setup(u => u.CategoryRepository.GetByIdAsync(createProductDto.CategoryId))
						   .ReturnsAsync(mockCategory);
			_mockUnitOfWork.Setup(u => u.ProductRepository.AddAsync(createProductDto))
						   .ReturnsAsync(mockProduct);

			// Act
			var result = await _controller.AddNewProduct(createProductDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var productDto = Assert.IsType<ResponseProductDto>(okResult.Value);
			Assert.Equal(mockProduct.Id, productDto.Id);
		}

		[Fact]
		[Trait("Category", "Product_AddServices")]
		public async Task AddNewProduct_ReturnsBadRequest_WhenCategoryDoesNotExist()
		{
			// Arrange
			var createProductDto = new CreateProductDto
			{
				Name = "Test Product",
				Description = "Test Description",
				Price = 10.99m,
				CategoryId = 999, // Non-existent category ID
			};

			_mockUnitOfWork.Setup(u => u.CategoryRepository.GetByIdAsync(createProductDto.CategoryId))
						   .ReturnsAsync((Category)null); // Simulate category not found

			// Act
			var result = await _controller.AddNewProduct(createProductDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			var response = Assert.IsType<BaseCommonResponse>(badRequestResult.Value);
			Assert.Equal(404, response.StatusCode);
			Assert.Equal($"This category id [{createProductDto.CategoryId}] not exist", response.Message);
		}

		[Fact]
		[Trait("Category", "Product_AddServices")]
		public async Task AddNewProduct_ReturnsBadRequest_WhenProductAdditionFails()
		{
			// Arrange
			var createProductDto = new CreateProductDto
			{
				Name = "Test Product",
				Description = "Test Description",
				Price = 10.99m,
				CategoryId = 1, // Valid category ID
			};

			_mockUnitOfWork.Setup(u => u.CategoryRepository.GetByIdAsync(createProductDto.CategoryId))
						   .ReturnsAsync(new Category { Id = 1, Name = "Test Category" });
			_mockUnitOfWork.Setup(u => u.ProductRepository.AddAsync(createProductDto))
						   .ReturnsAsync((Product)null); // Simulate product addition failed

			// Act
			var result = await _controller.AddNewProduct(createProductDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			var response = Assert.IsType<BaseCommonResponse>(badRequestResult.Value);
			Assert.Equal(400, response.StatusCode);
		}

		[Fact]
		[Trait("Category", "Product_UpdateServices")]
		public async Task UpdateExistingProductById_ReturnsOkObjectResult_WhenCategoryExistsAndProductUpdatedSuccessfully()
		{
			// Arrange
			var productId = 1;
			var updateProductDto = new UpdateProductDto
			{
				Id = productId,
				Name = "Test Product",
				Description = "Test Description",
				Price = 10.99m,
				CategoryId = 1, // Valid category ID
			};
			var mockCategory = new Category { Id = 1, Name = "Test Category" };
			var mockProduct = new Product { Id = productId, Name = "Test Product", Category = mockCategory };

			_mockUnitOfWork.Setup(u => u.CategoryRepository.GetByIdAsync(updateProductDto.CategoryId))
						   .ReturnsAsync(mockCategory);
			_mockUnitOfWork.Setup(u => u.ProductRepository.UpdateAsync(productId, updateProductDto))
						   .ReturnsAsync(mockProduct);

			// Act
			var result = await _controller.UpdateExistingProductById(productId, updateProductDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var productDto = Assert.IsType<ResponseProductDto>(okResult.Value);
			Assert.Equal(productId, productDto.Id);
		}

		[Fact]
		[Trait("Category", "Product_UpdateServices")]
		public async Task UpdateExistingProductById_ReturnsBadRequest_WhenCategoryDoesNotExist()
		{
			// Arrange
			var productId = 1;
			var updateProductDto = new UpdateProductDto
			{
				Id = productId,
				Name = "Test Product",
				Description = "Test Description",
				Price = 10.99m,
				CategoryId = 999, // Non-existent category ID
			};

			_mockUnitOfWork.Setup(u => u.CategoryRepository.GetByIdAsync(updateProductDto.CategoryId))
						   .ReturnsAsync((Category)null); // Simulate category not found

			// Act
			var result = await _controller.UpdateExistingProductById(productId, updateProductDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			var response = Assert.IsType<BaseCommonResponse>(badRequestResult.Value);
			Assert.Equal(404, response.StatusCode);
			Assert.Equal($"This category id [{updateProductDto.CategoryId}] not exist", response.Message);
		}

		[Fact]
		[Trait("Category", "Product_UpdateServices")]
		public async Task UpdateExistingProductById_ReturnsBadRequest_WhenProductUpdateFails()
		{
			// Arrange
			var productId = 1;
			var updateProductDto = new UpdateProductDto
			{
				Id = productId,
				Name = "Test Product",
				Description = "Test Description",
				Price = 10.99m,
				CategoryId = 1, // Valid category ID
			};

			_mockUnitOfWork.Setup(u => u.CategoryRepository.GetByIdAsync(updateProductDto.CategoryId))
						   .ReturnsAsync(new Category { Id = 1, Name = "Test Category" });
			_mockUnitOfWork.Setup(u => u.ProductRepository.UpdateAsync(productId, updateProductDto))
						   .ReturnsAsync((Product)null); // Simulate product update failed

			// Act
			var result = await _controller.UpdateExistingProductById(productId, updateProductDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			var response = Assert.IsType<BaseCommonResponse>(badRequestResult.Value);
			Assert.Equal(400, response.StatusCode);
		}

		[Fact]
		[Trait("Category", "Product_DeleteServices")]
		public async Task Delete_ReturnsOkResult_WhenProductDeletedSuccessfully()
		{
			// Arrange
			var productId = 1;
			_mockUnitOfWork.Setup(u => u.ProductRepository.DeleteWithPictureAsync(productId))
						   .ReturnsAsync(true); // Simulate successful deletion

			// Act
			var result = await _controller.Delete(productId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var response = Assert.IsType<bool>(okResult.Value);
			Assert.True(response);
		}

		[Fact]
		[Trait("Category", "Product_DeleteServices")]
		public async Task Delete_ReturnsNotFound_WhenProductNotFound()
		{
			// Arrange
			var productId = 999; // Non-existent product ID
			_mockUnitOfWork.Setup(u => u.ProductRepository.DeleteWithPictureAsync(productId))
						   .ReturnsAsync(false); // Simulate deletion failure

			// Act
			var result = await _controller.Delete(productId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			var response = Assert.IsType<BaseCommonResponse>(notFoundResult.Value);
			Assert.Equal(404, response.StatusCode);
			Assert.Equal($"this id [{productId}] Not Found", response.Message);
		}
	}
}

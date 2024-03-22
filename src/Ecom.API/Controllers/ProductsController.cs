using Ecom.API.Errors;
using Ecom.API.Helper;
using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize] //Only Authorized Can Consume
	public class ProductsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _config;

		public ProductsController(IUnitOfWork unitOfWork, IConfiguration config)
		{
			_unitOfWork = unitOfWork;
			_config = config;
		}

		[HttpGet("get-all-products")]
		[ProducesResponseType(StatusCodes.Status200OK)]                                     //possible HTTP response types
		[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status400BadRequest)] //possible HTTP response types
		[AllowAnonymous]                                                                    //Anyone can call (Non Authorized)
		[ResponseCache(Duration = 60)]                                                      // Cache for 60 seconds
		public async Task<ActionResult> GetAllProducts([FromQuery] ProductParams productParam)
		{
			if (productParam.PageNumber <= 0) return BadRequest(new BaseCommonResponse(400, "Invalid parameters"));

			var allProducts = await _unitOfWork.ProductRepository.GetAllAsync(productParam);
			var totalCount = await _unitOfWork.ProductRepository.CountAsync();

			//mapping to ProductDto
			var responseProductDtoList = allProducts.Select(f => new ResponseProductDto()
			{
				Id = f.Id,
				Name = f.Name,
				Description = f.Description,
				Price = f.Price,
				CategorName = f.Category.Name,
				ProductPicture = !string.IsNullOrEmpty(f.ProductPicture) ? _config["PictureServerUrl"] + f.ProductPicture : null
			}).ToList();

			// Return an OK response with paginated product data
			return Ok(new Pagination<ResponseProductDto>(productParam.PageNumber, productParam.PageSize, totalCount, responseProductDtoList));
		}


		[HttpGet("get-product-by-id/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]                                     //possible HTTP response types
		[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status404NotFound)]   //possible HTTP response types
		[AllowAnonymous]                                                                    //Anyone can call (Non Authorized)
		[ResponseCache(Duration = 60)]                                                      // Cache for 60 seconds
		public async Task<ActionResult> GetProductById([FromRoute] int id)
		{
			var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, x => x.Category);
			if (product == null) return NotFound(new BaseCommonResponse(404));

			// Mapping Product to ResponseProductDto
			var result = new ResponseProductDto()
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				CategorName = product.Category.Name,
				ProductPicture = !string.IsNullOrEmpty(product.ProductPicture) ? _config["PictureServerUrl"] + product.ProductPicture : null
			};

			return Ok(result);
		}


		[HttpPost("add-new-product")]
		[ProducesResponseType(StatusCodes.Status200OK)]                                     //possible HTTP response types
		[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status400BadRequest)] //possible HTTP response types
		public async Task<ActionResult> AddNewProduct([FromForm] CreateProductDto createProductDto)
		{
			var category = await _unitOfWork.CategoryRepository.GetByIdAsync(createProductDto.CategoryId);
			if (category == null) return BadRequest(new BaseCommonResponse(404, $"This category id [{createProductDto.CategoryId}] not exist"));

			var product = await _unitOfWork.ProductRepository.AddAsync(createProductDto);
			if (product == null) return BadRequest(new BaseCommonResponse(400));

			// Mapping Product to ResponseProductDto
			var responseProductDto = new ResponseProductDto()
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				CategorName = product.Category.Name,
				ProductPicture = product.ProductPicture
			};

			return Ok(responseProductDto);
		}


		[HttpPut("update-exiting-product-by-id/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]                                     //possible HTTP response types
		[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status404NotFound)]   //possible HTTP response types
		[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status400BadRequest)] //possible HTTP response types
		public async Task<ActionResult> UpdateExistingProductById([FromRoute] int id, [FromForm] UpdateProductDto updateProductDto)
		{
			var category = await _unitOfWork.CategoryRepository.GetByIdAsync(updateProductDto.CategoryId);
			if (category == null) return BadRequest(new BaseCommonResponse(404, $"This category id [{updateProductDto.CategoryId}] not exist"));

			var product = await _unitOfWork.ProductRepository.UpdateAsync(id, updateProductDto);
			if (product == null) return BadRequest(new BaseCommonResponse(400));

			// Mapping Product to ResponseProductDto
			var responseProductDto = new ResponseProductDto()
			{
				Id = product.Id,
				Name = updateProductDto.Name,
				Description = updateProductDto.Description,
				Price = updateProductDto.Price,
				CategorName = category.Name,
				ProductPicture = product.ProductPicture
			};

			return Ok(responseProductDto);
		}

		[HttpDelete("delete-product-by-id/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]                                     //possible HTTP response types
		[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status404NotFound)]   //possible HTTP response types
		public async Task<ActionResult> Delete([FromRoute] int id)
		{
			var response = await _unitOfWork.ProductRepository.DeleteWithPictureAsync(id);
			if (response == false) return NotFound(new BaseCommonResponse(404, $"this id [{id}] Not Found"));

			return Ok(response);
		}

	}
}

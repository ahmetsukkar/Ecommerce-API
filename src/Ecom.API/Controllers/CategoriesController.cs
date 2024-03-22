using Ecom.API.Errors;
using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Ecom.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize] //Only Authorized Can Consume
	public class CategoriesController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public CategoriesController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet("get-all-categories")]
		[AllowAnonymous]
		public async Task<ActionResult> GetAllCategotries()
		{
			var allCategories = await _unitOfWork.CategoryRepository.GetAllAsync();
			if (allCategories == null) return BadRequest(new BaseCommonResponse(500));

			if (allCategories.Any())
			{
				var responseCategoryDto = allCategories.Select(f => new ResponseCategoryDto
				{
					Id = f.Id,
					Name = f.Name,
					Discription = f.Description
				}).ToList();

				return Ok(responseCategoryDto);
			}
			else
			{
				return NotFound(new BaseCommonResponse(404));
			}
		}

		[HttpGet("get-category-by-id/{id}")]
		[AllowAnonymous]
		public async Task<ActionResult> GetCategoryById([FromRoute] int id)
		{
			var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
			if (category == null)
				return BadRequest(new BaseCommonResponse(404));

			var responseCategoryDto = new ResponseCategoryDto
			{
				Id = category.Id,
				Name = category.Name,
				Discription = category.Description
			};

			return Ok(responseCategoryDto);
		}

		[HttpPost("add-new-category")]
		public async Task<ActionResult> AddNewCategory([FromBody] CategoryDto categoryDto)
		{
			var newCategory = new Category()
			{
				Name = categoryDto.Name,
				Description = categoryDto.Discription
			};

			await _unitOfWork.CategoryRepository.AddAsync(newCategory);
			return Ok(categoryDto);
		}

		[HttpPut("update-exiting-category-by-id")]
		public async Task<ActionResult> UpdateExistingCategory([FromBody] UpdateCategoryDto categoryDto)
		{
			var exitingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryDto.Id);

			if (exitingCategory == null) return NotFound(new BaseCommonResponse(404));

			//Updating
			exitingCategory.Name = categoryDto.Name;
			exitingCategory.Description = categoryDto.Discription;
			await _unitOfWork.CategoryRepository.UpdateAsync(categoryDto.Id, exitingCategory);
			return Ok(categoryDto);
		}

		[HttpDelete("delete-category-by-id/{id}")]
		public async Task<ActionResult> DeleteCategory([FromRoute] int id)
		{
			var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
			if (category == null) return BadRequest(new BaseCommonResponse(404));

			var response = await _unitOfWork.CategoryRepository.DeleteAsync(id);
			if (response == false) return NotFound(new BaseCommonResponse(404));

			return Ok(response);
		}

	}
}

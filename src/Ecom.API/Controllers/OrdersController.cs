using Ecom.API.Errors;
using Ecom.API.Extensions;
using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecom.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class OrdersController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IOrderServices _orderServices;

		public OrdersController(UserManager<AppUser> userManager, IOrderServices orderServices)
		{
			_userManager = userManager;
			_orderServices = orderServices;
		}
	
		[HttpPost("create-order")]  //PurchaseEndpoint
		public async Task<IActionResult> CreateOrders([FromBody] OrderDto orderDto)
		{
			// Attempt to create an order by sending (CustomerEmail & BasketItems)
			var orderResponseDto = await _orderServices.CreateOrderAsync(orderDto.CustomerEmail, orderDto.BasketItems);

			// Check if the order creation was unsuccessful
			if (orderResponseDto == null) return BadRequest(new BaseCommonResponse(400, "Error While Creating Order"));

			// Return an OK response with the created order details
			return Ok(orderResponseDto);
		}

		[HttpGet("get-orders-for-current-user")]
		public async Task<IActionResult> GetOrdersForCurrentUser()
		{
			// Retrieve the current user
			var user = await _userManager.FindEmailByClaimPrincipleAsync(HttpContext.User);

			// Retrieve orders associated with the current user
			var resutl = await _orderServices.GetOrdersForUserAsync(user.Email);

			// Return an OK response with the retrieved orders
			return Ok(resutl);
		}

		[HttpGet("get-orders-for-current-user-by-id/{orderId}")]
		public async Task<IActionResult> GetOrdersForCurrentUserById([FromRoute] int orderId)
		{
			// Retrieve the current user
			var user = await _userManager.FindEmailByClaimPrincipleAsync(HttpContext.User);

			// Retrieve the specific order associated with the current user
			var resutl = await _orderServices.GetOrderByIdAsync(orderId, user.Email);

			// Check if the order was not found
			if (resutl == null) return NotFound(new BaseCommonResponse(404));

			// Return an OK response with the retrieved order details
			return Ok(resutl);
		}

		[HttpGet("get-orders-by-user-email/{email}")]
		public async Task<IActionResult> GetOrdersForCurrentUser([FromRoute] string email)
		{
			// Retrieve orders associated with the provided email
			var orders = await _orderServices.GetOrdersForUserAsync(email);

			// Return an OK response with the retrieved orders
			return Ok(orders);
		}

	}
}
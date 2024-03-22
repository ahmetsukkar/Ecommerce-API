using Ecom.Core.Dto;
using Ecom.Core.Entities;
using Ecom.Core.Entities.Orders;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Services
{
	public class OrderServices : IOrderServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public OrderServices(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_context = context;
		}

		public async Task<OrderResponseDto> CreateOrderAsync(string customerEmail, List<BasketItem> basketItems)
		{
			var customer = await _userManager.FindByEmailAsync(customerEmail);
			if (customer == null) return null;

			//Configure OrderItems
			var items = new List<OrderItem>();
			foreach (var item in basketItems)
			{
				var productItem = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
				var productItemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.ProductPicture);
				var orderItem = new OrderItem(productItemOrdered, item.Price, item.Quantity);
				items.Add(orderItem);
			}

			await _context.OrderItems.AddRangeAsync(items);
			await _context.SaveChangesAsync();

			// Calculate TotalAmount
			var totalAmount = items.Sum(o => o.Price * o.Quantity);

			// Check CustomerClassification, Set discountPercentage
			var customerClassification = await GetCustomerClassification(customer);
			var discountPercentage = customerClassification switch
			{
				CustomerClassification.Regular => 0.0m,
				CustomerClassification.Premium => 0.10m,
				CustomerClassification.Employee => 0.30m,
				_ => 0.0m,
			};

			// Set discountAmount & discountedTotal
			decimal discountAmount = totalAmount * discountPercentage;
			decimal discountedTotal = totalAmount - discountAmount;

			var order = new Order(customer.Id, customer.Email, items, totalAmount);
			if (order == null) return null;

			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();

			// Mapping to OrderResponseDto
			var OrderResponseDto = new OrderResponseDto()
			{
				OrginalPrice = totalAmount,
				DiscountedAmount = discountAmount,
				FinalPrice = discountedTotal,
				Order = order
			};

			return OrderResponseDto;
		}

		public async Task<Order> GetOrderByIdAsync(int id, string customerEmail)
		{
			var order = await _context.Orders
							  .Where(x => x.Id == id && x.CustomerEmail == customerEmail)
							  .Include(i => i.OrderItems)
							  .FirstOrDefaultAsync();

			return order;
		}

		public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string customerEmail)
		{
			var order = await _context.Orders
							 .Where(x => x.CustomerEmail == customerEmail)
							 .Include(i => i.OrderItems)
							 .ToListAsync();

			return order;
		}

		private async Task<CustomerClassification> GetCustomerClassification(AppUser customer)
		{
			var customerRoles = await _userManager.GetRolesAsync(customer);

			if (customerRoles.Contains("Employee"))
			{
				return CustomerClassification.Employee;
			}
			else
			{
				var previousMonthDate = DateTime.Now.AddMonths(-1);
				var customerOrdered = await (from or in _context.Orders
											 where or.CustomerId == customer.Id &&
											 or.OrderDate >= previousMonthDate
											 select or).ToListAsync();

				var previousMonthPurchaseTotalAmount = customerOrdered.Sum(s => s.TotalAmount);
				if (previousMonthPurchaseTotalAmount > 100)
					return CustomerClassification.Premium;
			}

			return CustomerClassification.Regular;
		}


	}
}

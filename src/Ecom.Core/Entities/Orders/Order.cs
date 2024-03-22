using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Entities.Orders
{
	public class Order : BaseEntity<int>
	{
		public Order()
		{

		}
		public Order(string customerId, string customerEmail, List<OrderItem> orderItems, decimal totalAmount)
		{
			CustomerId = customerId;
			CustomerEmail = customerEmail;
			OrderItems = orderItems;
			TotalAmount = totalAmount;
		}

		public string CustomerId { get; set; }
		public string CustomerEmail { get; set; }
		public DateTime OrderDate { get; set; } = DateTime.Now;
		public List<OrderItem> OrderItems { get; set; }
		public decimal TotalAmount { get; set; }
		public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;


		public decimal GetTotal()
		{
			return TotalAmount;
		}
	}
}

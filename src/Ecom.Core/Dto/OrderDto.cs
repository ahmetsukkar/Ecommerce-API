using Ecom.Core.Entities.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Dto
{
	public class OrderDto
	{
		[Required]
		public string CustomerEmail { get; set; }
		[Required]
		public List<BasketItem> BasketItems { get; set; }
	}
}

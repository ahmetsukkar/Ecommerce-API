using Ecom.Core.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Dto
{
	public class OrderResponseDto
	{
		public decimal OrginalPrice { get; set; }
		public decimal DiscountedAmount { get; set; }
		public decimal FinalPrice { get; set; }
		public Order Order { get; set; }
	}
}

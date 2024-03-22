using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Entities.Orders
{
    public class BasketItem
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

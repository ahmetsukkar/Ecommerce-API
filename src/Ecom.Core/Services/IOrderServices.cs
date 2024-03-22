using Ecom.Core.Dto;
using Ecom.Core.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Services
{
    public interface IOrderServices
    {
        Task<OrderResponseDto> CreateOrderAsync(string customerEmail, List<BasketItem> basketItems);

        Task<IEnumerable<Order>> GetOrdersForUserAsync(string customerEmail);

        Task<Order> GetOrderByIdAsync(int id, string customerEmail);
    }
}

using Microsoft.EntityFrameworkCore;
using OrderService.API.Entities;

namespace OrderService.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;
        public OrderRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var orderList = await _orderDbContext.Orders.ToListAsync();
            return orderList;
        }

        public async Task CreateOrder(Order order)
        {
            _orderDbContext.Orders.Add(order);
            await _orderDbContext.SaveChangesAsync();
            await Task.CompletedTask;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _orderDbContext.Orders.SingleAsync(s => s.Id == id);
        }

        public async Task EventOccured(Order order, string evt)
        {
            _orderDbContext.Orders.Single(p => p.Id == order.Id).Name = $"{order.Name} evt: {evt}";
            await Task.CompletedTask;
        }
    }
}

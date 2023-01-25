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

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int take, int skip)
        {
            var orderList =
                await _orderDbContext
                .Orders
                .Include("OrderItems")
                .Include("OrderItems.Product")
                .OrderByDescending(o => o.CreatedDate)
                .Take(take)
                .Skip(skip)
                .ToListAsync();
            return orderList;
        }

        public async Task CreateOrderAsync(Order order)
        {
            // _orderDbContext.OrderItems.Add(order.OrderItems);
            _orderDbContext.Orders.Add(order);
            await _orderDbContext.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _orderDbContext.Entry(order).State = EntityState.Modified;
            await _orderDbContext.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _orderDbContext
                .Orders
                .Include("OrderItems")
                .Include("OrderItems.Product")
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task EventOccuredAsync(Order order, string evt)
        {
            //TODO::Move to a product repository and update the product inventory
            _orderDbContext.Orders.Single(p => p.Id == order.Id).CustomerName = $"{order.CustomerName} evt: {evt}";
            await Task.CompletedTask;
        }
    }
}

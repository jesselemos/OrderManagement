using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;
        public OrderRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }
        public async Task CreateOrderAsync(Order order)
        {
            _orderDbContext.Orders.Add(order);
            await _orderDbContext.SaveChangesAsync();
        }
        public async Task UpdateOrderAsync(Order order)
        {
            _orderDbContext.Entry(order).State = EntityState.Modified;
            _orderDbContext.Entry(order).CurrentValues.SetValues(order);
            await _orderDbContext.SaveChangesAsync();
        }
        public async Task UpdateOrderItemsAsync(Order order, List<OrderItem> newItems)
        {
            _orderDbContext.Entry(order).State = EntityState.Modified;
            _orderDbContext.Entry(order).CurrentValues.SetValues(order);

            foreach (var existingItem in order.OrderItems.ToList())
            {
                if (!newItems.Any(c => c.Product?.Id == existingItem.Product?.Id))
                {
                    _orderDbContext.OrderItems.Remove(existingItem);
                }
            }

            foreach (var newItem in newItems)
            {
                var existingItem = order.OrderItems.SingleOrDefault(c => c.Product?.Id == newItem.Product?.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity = newItem.Quantity;
                    _orderDbContext.Entry(existingItem).CurrentValues.SetValues(existingItem);
                }
                else
                {
                    order.OrderItems.Add(newItem);
                }
            }

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
    }
}

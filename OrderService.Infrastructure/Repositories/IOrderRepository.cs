using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> GetAllOrdersAsync(int take, int skip);
        public Task CreateOrderAsync(Order order);
        public Task<Order?> GetOrderByIdAsync(Guid id);
        public Task UpdateOrderAsync(Order order);
        public Task UpdateOrderItemsAsync(Order order, List<OrderItem> newItems);
    }
}

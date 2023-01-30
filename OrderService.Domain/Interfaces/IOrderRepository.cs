using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces
{
    public interface IOrderRepository
    {
        public Task CreateOrderAsync(Order order);
        public Task UpdateOrderAsync(Order order);
        public Task UpdateOrderItemsAsync(Order order, List<OrderItem> newItems);
        public Task<Order?> GetOrderByIdAsync(Guid id);
        public Task<IEnumerable<Order>> GetAllOrdersAsync(int take, int skip);
    }
}

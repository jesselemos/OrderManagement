using OrderService.API.Entities;

namespace OrderService.API.Repositories
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> GetAllOrdersAsync();
        public Task CreateOrderAsync(Order order);
        public Task<Order> GetOrderByIdAsync(Guid id);
        public Task UpdateOrderAsync(Order order);
        public Task EventOccuredAsync(Order order, string evt);
    }
}

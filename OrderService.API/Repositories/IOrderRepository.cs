using OrderService.API.Entities;

namespace OrderService.API.Repositories
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> GetAllOrders();
        public Task CreateOrder(Order order);
        public Task<Order> GetOrderById(Guid id);
        public Task EventOccured(Order order, string evt);
    }
}

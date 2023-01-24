using OrderService.API.Entities;

namespace OrderService.API.Repositories
{
    public class FakeDataStore
    {
        private static List<Order> _orders;

        public FakeDataStore()
        {
            _orders = new List<Order>
            {
                new Order { Id = 1},
                new Order { Id = 2},
                new Order { Id = 3}
            };
        }

        public async Task AddOrder(Order order)
        {
            _orders.Add(order);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Order>> GetAllOrders() => await Task.FromResult(_orders);

        public async Task<Order> GetOrderById(int id) =>
            await Task.FromResult(_orders.Single(p => p.Id == id));

        public async Task EventOccured(Order order, string evt)
        {
            _orders.Single(p => p.Id == order.Id).Name = $"{order.Name} evt: {evt}";
            await Task.CompletedTask;
        }
    }
}

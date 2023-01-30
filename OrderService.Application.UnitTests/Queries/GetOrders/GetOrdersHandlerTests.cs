using OrderService.Application.Queries.GetOrders;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.DataSeed;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.UnitTests.Queries.GetOrders
{
    [TestFixture]
    public class GetOrdersHandlerTests
    {
        private IOrderRepository _orderRepository;

        [SetUp]
        public async Task Setup()
        {
            _orderRepository = new OrderRepository(await DbSeed.GetInMemoryOrderDbContext());
        }

        [Test]
        public async Task GetOrderByIdHandlerReturnsCorrectQuantityPerPage()
        {
            var quantityToTake = 10;
            var orders = await new GetOrdersHandler(_orderRepository).Handle(
                new GetOrdersQuery(quantityToTake, 0), new CancellationToken());

            Assert.Multiple(() =>
            {
                Assert.That(orders.Any());
                Assert.That(orders.Count, Is.EqualTo(quantityToTake));
            });
        }
    }
}
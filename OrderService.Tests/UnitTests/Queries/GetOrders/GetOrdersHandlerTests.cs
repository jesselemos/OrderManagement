using OrderService.Application.Queries.GetOrders;
using OrderService.Infrastructure.Repositories;
using OrderService.API.Tests.UnitTests.Helpers;

namespace OrderService.API.Tests.UnitTests.Queries.GetOrders
{
    [TestFixture]
    public class GetOrdersHandlerTests
    {
        private IOrderRepository _orderRepository;

        [SetUp]
        public void Setup()
        {
            _orderRepository = new OrderRepository(DatabaseHelper.GetOrderDbContext());
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
using OrderService.API.Queries.GetOrderById;
using OrderService.API.Repositories;
using OrderService.API.Tests.UnitTests.Helpers;

namespace OrderService.API.Tests.UnitTests.Queries
{
    [TestFixture]
    public class GetOrderByIdHandlerTests
    {
        private IOrderRepository _orderRepository;

        [SetUp]
        public void Setup()
        {
            _orderRepository = new OrderRepository(DatabaseHelper.GetOrderDbContext());
        }

        [Test]
        public async Task GetOrderByIdHandlerReturnsCorrectOrder()
        {
            var order = await new GetOrderByIdHandler(_orderRepository).Handle(
                new GetOrderByIdQuery(DatabaseHelper.OrderSeedId), new CancellationToken());

            var orderToMatch = await _orderRepository.GetOrderByIdAsync(DatabaseHelper.OrderSeedId);

            Assert.Multiple(() =>
            {
                Assert.That(order, Is.Not.Null);
                Assert.That(order?.Id, Is.EqualTo(orderToMatch?.Id));
            });
        }

        [Test]
        public async Task GetOrderByIdHandlerReturnsNullWhenOrderIdIsInvalid()
        {
            var order = await new GetOrderByIdHandler(_orderRepository).Handle(
                new GetOrderByIdQuery(Guid.NewGuid()), new CancellationToken()); ;

            Assert.That(order, Is.Null);
        }
    }
}
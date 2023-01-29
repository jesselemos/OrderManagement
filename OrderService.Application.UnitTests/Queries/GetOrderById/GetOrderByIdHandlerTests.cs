using OrderService.Application.Queries.GetOrderById;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.DataSeed;

namespace OrderService.Application.UnitTests.Queries.GetOrderById
{
    [TestFixture]
    public class GetOrderByIdHandlerTests
    {
        private IOrderRepository _orderRepository;

        [SetUp]
        public async Task Setup()
        {
            _orderRepository = new OrderRepository(await DbSeed.GetInMemoryOrderDbContext());
        }

        [Test]
        public async Task GetOrderByIdHandlerReturnsCorrectOrder()
        {
            var order = await new GetOrderByIdHandler(_orderRepository).Handle(
                new GetOrderByIdQuery(DbSeed.OrderSeedId), new CancellationToken());

            var orderToMatch = await _orderRepository.GetOrderByIdAsync(DbSeed.OrderSeedId);

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
                new GetOrderByIdQuery(Guid.NewGuid()), new CancellationToken());

            Assert.That(order, Is.Null);
        }
    }
}
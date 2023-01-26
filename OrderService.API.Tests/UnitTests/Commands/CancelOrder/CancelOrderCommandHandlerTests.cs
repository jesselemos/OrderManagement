using AutoMapper;
using NSubstitute;
using OrderService.API.Commands.CancelOrder;
using OrderService.API.Entities;
using OrderService.API.Repositories;
using OrderService.API.Tests.UnitTests.Helpers;

namespace OrderService.API.Tests.UnitTests.Commands.CancelOrder
{
    [TestFixture]
    public class CancelOrderCommandHandlerTests
    {
        private IOrderRepository _orderRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = AutoMapperHelper.CreateMapper();
            _orderRepository = new OrderRepository(DatabaseHelper.GetOrderDbContext());
        }

        [Test]
        public async Task CanCancelOrderAndSetStatusCanceled()
        {
            var result = await new CancelOrderCommandHandler(_orderRepository, _mapper).Handle(
                new CancelOrderCommand()
                {
                    OrderId = DatabaseHelper.OrderSeedId
                }, new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DatabaseHelper.OrderSeedId);

            Assert.Multiple(() =>
            {
                Assert.That(order, Is.Not.Null);
                Assert.That(order?.OrderStatus == OrderStatus.Canceled);
            });
        }

        [Test]
        public void ThrowExceptionIfOrderNotExistsInDatabase()
        {
            var commandHandler = new CancelOrderCommandHandler(Substitute.For<IOrderRepository>(), _mapper);
            Guid orderId = new();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new CancelOrderCommand()
                    {
                        OrderId = orderId
                    }, new CancellationToken()),
            Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"OrderId: {orderId} not found in our database."));
        }
    }
}
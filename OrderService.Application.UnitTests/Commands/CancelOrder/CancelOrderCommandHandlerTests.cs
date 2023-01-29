using AutoMapper;
using NSubstitute;
using OrderService.Application.Commands.CancelOrder;
using OrderService.Application.Mappers;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Helpers;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.UnitTests.Commands.CancelOrder
{
    [TestFixture]
    public class CancelOrderCommandHandlerTests
    {
        private IOrderRepository _orderRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = AutoMapping.CreateMapper();
            _orderRepository = new OrderRepository(DatabaseHelper.GetOrderDbContext());
        }

        [Test]
        public async Task CanCancelOrderAndSetStatusCanceled()
        {
            await new CancelOrderCommandHandler(_orderRepository, _mapper).Handle(
                new CancelOrderCommand()
                {
                    OrderId = DatabaseHelper.OrderSeedId
                }, new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DatabaseHelper.OrderSeedId);

            Assert.Multiple(() =>
            {
                Assert.That(order, Is.Not.Null);
                Assert.That(order?.OrderStatus, Is.EqualTo(OrderStatus.Canceled));
            });
        }

        [Test]
        public void ThrowExceptionIfOrderNotExistsInDatabase()
        {
            var commandHandler = new CancelOrderCommandHandler(Substitute.For<IOrderRepository>(), _mapper);
            var orderId = Guid.NewGuid();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new CancelOrderCommand()
                    {
                        OrderId = orderId
                    }, new CancellationToken()),
            Throws.TypeOf<ArgumentNullException>()
                        .With.Message.Contains($"OrderId: {orderId} not found in our database."));
        }
    }
}
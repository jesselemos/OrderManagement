using AutoMapper;
using NSubstitute;
using OrderService.Application.Commands.UpdateOrderAddress;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.DataSeed;
using OrderService.Application.Mappers;

namespace OrderService.Application.UnitTests.Commands.UpdateOrderAddress
{
    [TestFixture]
    public class UpdateOrderAddressCommandHandlerTests
    {
        private IOrderRepository _orderRepository;
        private IMapper _mapper;

        [SetUp]
        public async Task Setup()
        {
            _mapper = AutoMapping.CreateMapper();
            _orderRepository = new OrderRepository(await DbSeed.GetInMemoryOrderDbContext());
        }

        [Test]
        public async Task CanUpdateOrderAddressAndSetStatusCreated()
        {
            await new UpdateOrderAddressCommandHandler(_orderRepository, _mapper).Handle(
                new UpdateOrderAddressCommand()
                {
                    OrderId = DbSeed.OrderSeedId,
                    AddressLine = "New AddressLine",
                    AddressName = "New AddressName",
                    EirCode = "NewCode",
                    County = "New County",
                }, new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DbSeed.OrderSeedId) ?? new();

            Assert.Multiple(() =>
            {
                Assert.That(order, Is.Not.Null);
                Assert.That(order.AddressLine, Is.EqualTo("New AddressLine"));
                Assert.That(order.AddressName, Is.EqualTo("New AddressName"));
                Assert.That(order.EirCode, Is.EqualTo("NewCode"));
                Assert.That(order.County, Is.EqualTo("New County"));
            });
        }

        [Test]
        public void ThrowExceptionIfOrderNotExistsInDatabase()
        {
            var commandHandler = new UpdateOrderAddressCommandHandler(Substitute.For<IOrderRepository>(), _mapper);
            var orderId = Guid.NewGuid();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new UpdateOrderAddressCommand()
                    {
                        OrderId = orderId,
                        AddressLine = "AddressLine",
                        AddressName = "AddressName",
                        EirCode = "EirCode",
                        County = "County",
                    }, new CancellationToken()),
                        Throws.TypeOf<ArgumentNullException>()
                        .With.Message.Contains($"OrderId: {orderId} not found in our database."));
        }
    }
}
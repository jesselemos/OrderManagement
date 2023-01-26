using AutoMapper;
using NSubstitute;
using OrderService.API.Commands.UpdateOrderAddress;
using OrderService.API.Repositories;
using OrderService.API.Tests.UnitTests.Helpers;

namespace OrderService.API.Tests.UnitTests.Commands.UpdateOrderAddress
{
    [TestFixture]
    public class UpdateOrderAddressCommandHandlerTests
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
        public async Task CanUpdateOrderAddressAndSetStatusCreated()
        {
            var result = await new UpdateOrderAddressCommandHandler(_orderRepository, _mapper).Handle(
                new UpdateOrderAddressCommand()
                {
                    OrderId = DatabaseHelper.OrderSeedId,
                    AddressLine = "New AddressLine",
                    AddressName = "New AddressName",
                    EirCode = "NewCode",
                    County = "New County",
                }, new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DatabaseHelper.OrderSeedId);

            Assert.Multiple(() =>
            {
                Assert.That(order, Is.Not.Null);
                Assert.That(order?.AddressLine == "New AddressLine");
                Assert.That(order?.AddressName == "New AddressName");
                Assert.That(order?.EirCode == "NewCode");
                Assert.That(order?.County == "New County");
            });
        }

        [Test]
        public void ThrowExceptionIfOrderNotExistsInDatabase()
        {
            var commandHandler = new UpdateOrderAddressCommandHandler(Substitute.For<IOrderRepository>(), _mapper);
            Guid orderId = new();

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
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"OrderId: {orderId} not found in our database."));
        }
    }
}
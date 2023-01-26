using AutoMapper;
using NSubstitute;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Entities;
using OrderService.API.Models;
using OrderService.API.Repositories;
using OrderService.API.UnitTests.Helpers;

namespace OrderService.API.UnitTests.Commands.CreateOrder
{
    [TestFixture]
    public class CreateOrderCommandHandlerTests
    {
        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = AutoMapperHelper.CreateMapper();
            var orderDbContext = DatabaseHelper.GetOrderDbContext();
            _orderRepository = new OrderRepository(orderDbContext);
            _productRepository = new ProductRepository(orderDbContext);
        }

        [Test]
        public async Task CanCreateOrderAndSetStatusCreated()
        {
            var result = await new CreateOrderCommandHandler(_orderRepository, _productRepository, _mapper).Handle(
                new CreateOrderCommand()
                {
                    CustomerName = "Name",
                    AddressLine = "AddressLine",
                    AddressName = "AddressName",
                    EirCode = "EirCode",
                    County = "County",
                    OrderItems = new List<CreateOrderItem>
                    {
                        new CreateOrderItem
                        {
                            ProductId = DatabaseHelper.ProductSeedId,
                            Quantity = 10
                        }
                    }
                }, new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DatabaseHelper.OrderSeedId);

            Assert.Multiple(() =>
            {
                Assert.That(Guid.TryParse(result.ToString(), out _), Is.True);
                Assert.That(order, Is.Not.Null);
                Assert.That(order?.OrderStatus == OrderStatus.Created);
            });
        }

        [Test]
        public void ThrowExceptionIfProductNotExistsInDatabase()
        {
            var commandHandler = new CreateOrderCommandHandler(_orderRepository, Substitute.For<IProductRepository>(), _mapper);
            Guid productId = new();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new CreateOrderCommand()
                    {
                        CustomerName = "Name",
                        AddressLine = "AddressLine",
                        AddressName = "AddressName",
                        EirCode = "EirCode",
                        County = "County",
                        OrderItems = new List<CreateOrderItem>
                        {
                            new CreateOrderItem
                            {
                                ProductId = productId,
                                Quantity = 10
                            }
                        }
                    }, new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"ProductId: {productId} not found in our database."));
        }

        [Test]
        public async Task ThrowExceptionIfThereIsNotEnoughStockForTheProductAsync()
        {
            var commandHandler = new CreateOrderCommandHandler(_orderRepository, _productRepository, _mapper);
            var productId = DatabaseHelper.ProductSeedId;
            var product = await _productRepository.GetProductByIdAsync(productId);

            Assert.That(async () =>
                    await commandHandler.Handle(
                        new CreateOrderCommand()
                        {
                            CustomerName = "Name",
                            AddressLine = "AddressLine",
                            AddressName = "AddressName",
                            EirCode = "EirCode",
                            County = "County",
                            OrderItems = new List<CreateOrderItem>
                            {
                            new CreateOrderItem
                            {
                                ProductId = productId,
                                Quantity = 999
                            }
                            }
                        }, new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"There is only {product?.Stock} units available of {product?.Name}"));
        }
    }
}
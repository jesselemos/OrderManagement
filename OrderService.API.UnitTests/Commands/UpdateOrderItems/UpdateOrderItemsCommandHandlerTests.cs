using NSubstitute;
using OrderService.API.Commands.UpdateOrderItems;
using OrderService.API.Models;
using OrderService.API.Repositories;
using OrderService.API.UnitTests.Helpers;

namespace OrderService.API.UnitTests.Commands.UpdateOrderItems
{
    [TestFixture]
    public class UpdateOrderItemsCommandHandlerTests
    {
        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;

        [SetUp]
        public void Setup()
        {
            var orderDbContext = DatabaseHelper.GetOrderDbContext();
            _orderRepository = new OrderRepository(orderDbContext);
            _productRepository = new ProductRepository(orderDbContext);
        }

        [Test]
        public async Task CanUpdateOrderItems()
        {
            var result = await new UpdateOrderItemsCommandHandler(_orderRepository, _productRepository).Handle(
                new UpdateOrderItemsCommand()
                {
                    OrderId = DatabaseHelper.OrderSeedId,
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
                Assert.That(order, Is.Not.Null);
                Assert.That(order?.OrderItems.Count, Is.GreaterThan(0));
            });
        }

        [Test]
        public void ThrowExceptionIfOrderNotExistsInDatabase()
        {
            var commandHandler = new UpdateOrderItemsCommandHandler(Substitute.For<IOrderRepository>(), Substitute.For<IProductRepository>());
            Guid orderId = new();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new UpdateOrderItemsCommand()
                    {
                        OrderId = orderId
                    }, new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"OrderId: {orderId} not found in our database."));
        }

        [Test]
        public void ThrowExceptionIfProductNotExistsInDatabase()
        {
            var commandHandler = new UpdateOrderItemsCommandHandler(_orderRepository, Substitute.For<IProductRepository>());
            Guid productId = new();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new UpdateOrderItemsCommand()
                    {
                        OrderId = DatabaseHelper.OrderSeedId,
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
            var commandHandler = new UpdateOrderItemsCommandHandler(_orderRepository, _productRepository);
            var productId = DatabaseHelper.ProductSeedId;
            var product = await _productRepository.GetProductByIdAsync(productId);

            Assert.That(async () =>
                    await commandHandler.Handle(
                        new UpdateOrderItemsCommand()
                        {
                            OrderId = DatabaseHelper.OrderSeedId,
                            OrderItems = new List<CreateOrderItem>
                            {
                                new CreateOrderItem
                                {
                                    ProductId = productId,
                                    Quantity = 9999
                                }
                            }
                        }, new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"There is only {product?.Stock} units available of {product?.Name}"));
        }
    }
}
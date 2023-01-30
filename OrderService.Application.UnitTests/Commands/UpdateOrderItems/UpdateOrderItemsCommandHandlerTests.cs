using NSubstitute;
using OrderService.Application.Commands.UpdateOrderItems;
using OrderService.Domain.Interfaces;
using OrderService.Domain.Models;
using OrderService.Infrastructure.DataSeed;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.UnitTests.Commands.UpdateOrderItems
{
    [TestFixture]
    public class UpdateOrderItemsCommandHandlerTests
    {
        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;

        [SetUp]
        public async Task Setup()
        {
            var orderDbContext = await DbSeed.GetInMemoryOrderDbContext();
            _orderRepository = new OrderRepository(orderDbContext);
            _productRepository = new ProductRepository(orderDbContext);
        }

        [Test]
        public async Task CanUpdateOrderItems()
        {
            await new UpdateOrderItemsCommandHandler(_orderRepository, _productRepository).Handle(
                new UpdateOrderItemsCommand()
                {
                    OrderId = DbSeed.OrderSeedId,
                    OrderItems = new List<CreateOrderItem>
                    {
                        new CreateOrderItem
                        {
                            ProductId = DbSeed.ProductSeedId,
                            Quantity = 10
                        }
                    }
                }, new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DbSeed.OrderSeedId);

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
            var orderId = Guid.NewGuid();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new UpdateOrderItemsCommand()
                    {
                        OrderId = orderId
                    }, new CancellationToken()),
                        Throws.TypeOf<ArgumentNullException>()
                        .With.Message.Contains($"OrderId: {orderId} not found in our database."));
        }

        [Test]
        public void ThrowExceptionIfProductNotExistsInDatabase()
        {
            var commandHandler = new UpdateOrderItemsCommandHandler(_orderRepository, Substitute.For<IProductRepository>());
            var productId = Guid.NewGuid();

            Assert.That(async () =>
                await commandHandler.Handle(
                    new UpdateOrderItemsCommand()
                    {
                        OrderId = DbSeed.OrderSeedId,
                        OrderItems = new List<CreateOrderItem>
                        {
                            new CreateOrderItem
                            {
                                ProductId = productId,
                                Quantity = 10
                            }
                        }
                    }, new CancellationToken()),
                        Throws.TypeOf<ArgumentNullException>()
                        .With.Message.Contains($"ProductId: {productId} not found in our database."));
        }

        [Test]
        public async Task ThrowExceptionIfThereIsNotEnoughStockForTheProductAsync()
        {
            var commandHandler = new UpdateOrderItemsCommandHandler(_orderRepository, _productRepository);
            var productId = DbSeed.ProductSeedId;
            var product = await _productRepository.GetProductByIdAsync(productId);

            Assert.That(async () =>
                    await commandHandler.Handle(
                        new UpdateOrderItemsCommand()
                        {
                            OrderId = DbSeed.OrderSeedId,
                            OrderItems = new List<CreateOrderItem>
                            {
                                new CreateOrderItem
                                {
                                    ProductId = productId,
                                    Quantity = 9999
                                }
                            }
                        }, new CancellationToken()),
                        Throws.TypeOf<ArgumentOutOfRangeException>()
                        .With.Message.Contains($"There is only {product?.Stock} units available of {product?.Name}"));
        }
    }
}
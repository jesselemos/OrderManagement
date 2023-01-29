using NSubstitute;
using OrderService.Application.Commands.UpdateOrderItems;
using OrderService.Domain.Models;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.DataSeed;

namespace OrderService.Application.UnitTests.Commands.UpdateOrderItems
{
    [TestFixture]
    public class UpdateOrderItemsNotificationHandlerTests
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
        public async Task CanUpdateOrderItemsNotificationAndProductStockIsDecreased()
        {
            var productId = DbSeed.ProductSeedId;
            var product = await _productRepository.GetProductByIdAsync(productId);
            var oldStock = product?.Stock;
            await new UpdateOrderItemsNotificationHandler(_orderRepository, _productRepository).Handle(
                new UpdateOrderItemsNotification
                (new UpdateOrderItemsCommand()
                {
                    OrderId = DbSeed.OrderSeedId,
                }), new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DbSeed.OrderSeedId);
            var prodQuantity = order?.OrderItems.Find(f => f.Product?.Id == productId)?.Quantity;

            Assert.That(product?.Stock, Is.EqualTo(oldStock + prodQuantity));
        }

        [Test]
        public void ThrowExceptionIfOrderNotExistsInDatabase()
        {
            var handler = new UpdateOrderItemsNotificationHandler(_orderRepository, Substitute.For<IProductRepository>());
            var orderId = Guid.NewGuid();

            Assert.That(async () =>
                await handler.Handle(
                    new UpdateOrderItemsNotification(
                        new UpdateOrderItemsCommand()
                        {
                            OrderId = orderId
                        }), new CancellationToken()),
                        Throws.TypeOf<ArgumentNullException>()
                        .With.Message.Contains($"OrderId: {orderId} not found in our database."));
        }

        [Test]
        public void DoesNotThrowExceptionIfProductDoesNotExistsInDatabaseToUpdateTheOtherItems()
        {
            var handler = new UpdateOrderItemsNotificationHandler(_orderRepository, Substitute.For<IProductRepository>());

            Assert.That(async () =>
                await handler.Handle(
                    new UpdateOrderItemsNotification(
                        new UpdateOrderItemsCommand()
                        {
                            OrderId = DbSeed.OrderSeedId,
                            OrderItems = new List<CreateOrderItem>
                            {
                                new CreateOrderItem
                                {
                                    ProductId = Guid.Empty,
                                    Quantity = 1
                                }
                            }

                        }), new CancellationToken()),
                        Throws.Nothing);
        }
    }
}
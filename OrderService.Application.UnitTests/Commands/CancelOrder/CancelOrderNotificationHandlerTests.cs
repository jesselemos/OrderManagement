using NSubstitute;
using OrderService.Application.Commands.CancelOrder;
using OrderService.Infrastructure.DataSeed;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.UnitTests.Commands.CancelOrder
{
    [TestFixture]
    public class CancelOrderNotificationHandlerTests
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
        public async Task CanCancelOrderNotificationAndProductStockIsDecreased()
        {
            var productId = DbSeed.ProductSeedId;
            var product = await _productRepository.GetProductByIdAsync(productId);
            var oldStock = product?.Stock;
            await new CancelOrderNotificationHandler(_orderRepository, _productRepository).Handle(
                new CancelOrderNotification
                (new CancelOrderCommand()
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
            var handler = new CancelOrderNotificationHandler(_orderRepository, Substitute.For<IProductRepository>());
            var orderId = Guid.NewGuid();

            Assert.That(async () =>
                await handler.Handle(
                    new CancelOrderNotification(
                        new CancelOrderCommand()
                        {
                            OrderId = orderId,
                        }), new CancellationToken()),
                        Throws.TypeOf<ArgumentNullException>()
                        .With.Message.Contains($"OrderId: {orderId} not found in our database."));
        }

        [Test]
        public void DoesNotThrowExceptionIfProductDoesNotExistsInDatabaseToUpdateTheOtherItems()
        {
            var handler = new CancelOrderNotificationHandler(_orderRepository, Substitute.For<IProductRepository>());

            Assert.That(async () =>
                await handler.Handle(
                    new CancelOrderNotification(
                        new CancelOrderCommand()
                        {
                            OrderId = DbSeed.OrderSeedId,
                        }), new CancellationToken()),
                        Throws.Nothing);
        }
    }
}
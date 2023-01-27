using NSubstitute;
using OrderService.API.Commands.UpdateOrderItems;
using OrderService.API.Models;
using OrderService.API.Repositories;
using OrderService.API.Tests.UnitTests.Helpers;

namespace OrderService.API.Tests.UnitTests.Commands.UpdateOrderItems
{
    [TestFixture]
    public class UpdateOrderItemsNotificationHandlerTests
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
        public async Task CanUpdateOrderItemsNotificationAndProductStockIsDecreased()
        {
            var productId = DatabaseHelper.ProductSeedId;
            var product = await _productRepository.GetProductByIdAsync(productId);
            var oldStock = product?.Stock;
            await new UpdateOrderItemsNotificationHandler(_orderRepository, _productRepository).Handle(
                new UpdateOrderItemsNotification
                (new UpdateOrderItemsCommand()
                {
                    OrderId = DatabaseHelper.OrderSeedId,
                }), new CancellationToken());

            var order = await _orderRepository.GetOrderByIdAsync(DatabaseHelper.OrderSeedId);
            var prodQuantity = order?.OrderItems.Find(f => f.Product?.Id == productId)?.Quantity;

            Assert.That(product?.Stock, Is.EqualTo(oldStock + prodQuantity));
        }

        [Test]
        public void ThrowExceptionIfOrderNotExistsInDatabase()
        {
            var handler = new UpdateOrderItemsNotificationHandler(_orderRepository, Substitute.For<IProductRepository>());
            Guid orderId = new();

            Assert.That(async () =>
                await handler.Handle(
                    new UpdateOrderItemsNotification(
                        new UpdateOrderItemsCommand()
                        {
                            OrderId = orderId
                        }), new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"OrderId: {orderId} not found in our database."));
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
                            OrderId = DatabaseHelper.OrderSeedId,
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
using NSubstitute;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Domain.Models;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Helpers;

namespace OrderService.Application.UnitTests.Commands.CreateOrder
{
    [TestFixture]
    public class CreateOrderNotificationHandlerTests
    {
        private IProductRepository _productRepository;

        [SetUp]
        public void Setup()
        {
            _productRepository = new ProductRepository(DatabaseHelper.GetOrderDbContext());
        }

        [Test]
        public async Task CanCreateOrderNotificationAndProductStockIsDecreased()
        {
            var productId = DatabaseHelper.ProductSeedId;
            var orderQuantity = 2;
            var product = await _productRepository.GetProductByIdAsync(productId);
            var oldStock = product?.Stock;
            await new CreateOrderNotificationHandler(_productRepository).Handle(
                new CreateOrderNotification
                (new CreateOrderCommand()
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
                            Quantity = orderQuantity
                        }
                    }
                }), new CancellationToken());

            Assert.That(product?.Stock, Is.EqualTo(oldStock - orderQuantity));
        }

        [Test]
        public void ThrowExceptionIfProductNotExistsInDatabase()
        {
            var handler = new CreateOrderNotificationHandler(Substitute.For<IProductRepository>());
            var productId = Guid.NewGuid();

            Assert.That(async () =>
                await handler.Handle(
                    new CreateOrderNotification(
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
                        }), new CancellationToken()),
                        Throws.TypeOf<ArgumentNullException>()
                        .With.Message.Contains($"ProductId: {productId} not found in our database."));
        }

        [Test]
        public async Task ThrowExceptionIfThereIsNotEnoughStockForTheProductAsync()
        {
            var handler = new CreateOrderNotificationHandler(_productRepository);
            var productId = DatabaseHelper.ProductSeedId;
            var product = await _productRepository.GetProductByIdAsync(productId);

            Assert.That(async () =>
                    await handler.Handle(
                        new CreateOrderNotification(
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
                                        Quantity = 999999999
                                    }
                                }
                            }), new CancellationToken()),
                        Throws.TypeOf<ArgumentOutOfRangeException>()
                        .With.Message.Contains($"There is only {product?.Stock} units available of {product?.Name}"));
        }
    }
}
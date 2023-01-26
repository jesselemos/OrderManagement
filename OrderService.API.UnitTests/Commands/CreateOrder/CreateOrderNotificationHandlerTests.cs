using NSubstitute;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Models;
using OrderService.API.Repositories;
using OrderService.API.UnitTests.Helpers;

namespace OrderService.API.UnitTests.Commands.CreateOrder
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
            var productId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
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
            Guid productId = new();

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
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"ProductId: {productId} not found in our database"));
        }

        [Test]
        public async Task ThrowExceptionIfThereIsNotEnoughStockForTheProductAsync()
        {
            var handler = new CreateOrderNotificationHandler(_productRepository);
            var productId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
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
                                        Quantity = 999
                                    }
                                }
                            }), new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"There is only {product?.Stock} units available of {product?.Name}"));
        }
    }
}
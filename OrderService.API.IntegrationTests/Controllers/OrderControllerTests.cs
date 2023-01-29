using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using OrderService.Application.Commands.CancelOrder;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Commands.UpdateOrderAddress;
using OrderService.Application.Commands.UpdateOrderItems;
using OrderService.Domain.Entities;
using OrderService.Domain.Models;
using OrderService.Infrastructure.DataSeed;
using OrderService.Infrastructure.Repositories;
using System.Net;
using System.Text;

namespace OrderService.API.IntegrationTests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;
        private IProductRepository ProductRepository
        {
            get
            {
                var context = _factory.Services.CreateScope().ServiceProvider.GetService<OrderDbContext>();
                return new ProductRepository(context ?? DbSeed.GetInMemoryOrderDbContext().Result);
            }
        }

        [SetUp]
        public void Init()
        {
            _factory = new WebApplicationFactory<Program>();
            _httpClient = _factory.CreateClient();
        }

        [Test]
        public async Task CreateOrderSetsCreatedStatusAndDecreasesProductStock()
        {
            //Arrange
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductSeedId);
            var productTwoBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductTwoSeedId);
            var productThreeBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductThreeSeedId);

            // Act
            var orderId = await CreateOrderApi(3);

            //assert
            var productAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductSeedId);
            var productTwoAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductTwoSeedId);
            var productThreeAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductThreeSeedId);
            var productQuantityOrdered = 10;
            var order = await GetOrderApi(orderId);

            Assert.Multiple(() =>
            {
                Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Created));
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock - productQuantityOrdered));
                Assert.That(productTwoAfterOrder?.Stock, Is.EqualTo(productTwoBeforeOrder?.Stock - productQuantityOrdered));
                Assert.That(productThreeAfterOrder?.Stock, Is.EqualTo(productThreeBeforeOrder?.Stock - productQuantityOrdered));
            });
        }

        [Test]
        public async Task CancelOrderSetsCanceledStatusAndReturnsStockToTheProducts()
        {
            //Arrange
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductSeedId);
            var productTwoBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductTwoSeedId);
            var productThreeBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductThreeSeedId);
            var orderId = await CreateOrderApi();

            // Act
            var command = new CancelOrderCommand { OrderId = orderId };
            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var cancelResult = await _httpClient.PutAsync($"/api/v1/order/cancel", content);

            //assert
            var productAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductSeedId);
            var productTwoAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductTwoSeedId);
            var productThreeAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductThreeSeedId);
            var order = await GetOrderApi(orderId);

            Assert.Multiple(() =>
            {
                Assert.That(cancelResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Canceled));
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock));
                Assert.That(productTwoAfterOrder?.Stock, Is.EqualTo(productTwoBeforeOrder?.Stock));
                Assert.That(productThreeAfterOrder?.Stock, Is.EqualTo(productThreeBeforeOrder?.Stock));
            });
        }

        [Test]
        public async Task UpdateOrderAddressSetsTheAddressCorrectly()
        {
            //Arrange
            var orderId = await CreateOrderApi();
            var command = new UpdateOrderAddressCommand()
            {
                OrderId = orderId,
                AddressLine = "Apartment 7, New Address Line",
                AddressName = "New Work Address",
                EirCode = "V01HT01",
                County = "New County"
            };

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var result = await _httpClient.PutAsync($"/api/v1/order/address", content);

            //assert
            var order = await GetOrderApi(orderId);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Created));
                Assert.That(order.AddressLine, Is.EqualTo("Apartment 7, New Address Line"));
                Assert.That(order.AddressName, Is.EqualTo("New Work Address"));
                Assert.That(order.EirCode, Is.EqualTo("V01HT01"));
                Assert.That(order.County, Is.EqualTo("New County"));
            });
        }

        [Test]
        public async Task UpdateOrderItemsSetsTheItemsAndProductStocksCorrectly()
        {
            //Arrange
            var productQuantityToOrder = 10;
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductSeedId);
            var productTwoBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductTwoSeedId);
            var productThreeBeforeOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductThreeSeedId);
            var orderId = await CreateOrderApi(2);

            var productTwoNewQuantity = 2;
            // Act
            var command = new UpdateOrderItemsCommand()
            {
                OrderId = orderId,
                OrderItems = new List<CreateOrderItem>
                {
                    //productOne left out on purpose
                    new CreateOrderItem
                    {
                        ProductId = DbSeed.ProductTwoSeedId,
                        Quantity = productTwoNewQuantity
                    },
                    //new product
                    new CreateOrderItem
                    {
                        ProductId = DbSeed.ProductThreeSeedId,
                        Quantity = productQuantityToOrder
                    },
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var updateItemsResult = await _httpClient.PutAsync($"/api/v1/order/items", content);

            //asserting product stock
            var productAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductSeedId);
            var productTwoAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductTwoSeedId);
            var productThreeAfterOrder = await ProductRepository.GetProductByIdAsync(DbSeed.ProductThreeSeedId);
            Assert.Multiple(() =>
            {
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock));
                Assert.That(productTwoAfterOrder?.Stock, Is.EqualTo(productTwoBeforeOrder?.Stock - productTwoNewQuantity));
                Assert.That(productThreeAfterOrder?.Stock, Is.EqualTo(productThreeBeforeOrder?.Stock - productQuantityToOrder));
                Assert.That(updateItemsResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

            //asserting new orderItens are as expected
            var order = await GetOrderApi(orderId);

            Assert.Multiple(() =>
            {
                Assert.That(updateItemsResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(order.OrderItems, Has.Count.EqualTo(2));
                Assert.That(!order.OrderItems.Any(f => f.Product.Id == DbSeed.ProductSeedId));
                Assert.That(order.OrderItems.Single(f => f.Product.Id == DbSeed.ProductTwoSeedId).Quantity, Is.EqualTo(productTwoNewQuantity));
                Assert.That(order.OrderItems.Single(f => f.Product.Id == DbSeed.ProductThreeSeedId).Quantity, Is.EqualTo(productQuantityToOrder));
            });
        }

        [Test]
        public async Task CreateOrderWithInvalidRequestReturnsError()
        {
            //Arrange
            var content = new StringContent(JsonConvert.SerializeObject(
                new CreateOrderCommand()), Encoding.UTF8, "application/json");

            // Act
            var result = await _httpClient.PostAsync($"/api/v1/order", content);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            });
        }

        [Test]
        public async Task GetOrderByIdQueryReturnsCreatedOrder()
        {
            //Arrange
            var orderId = await CreateOrderApi();

            // Act
            var order = await GetOrderApi(orderId);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(order.OrderStatus, Is.EqualTo(OrderStatus.Created));
                Assert.That(order.Total, Is.EqualTo(60));
            });
        }

        [Test]
        public async Task GetOrdersQueryReturnsPaginatedOrders()
        {
            //Arrange
            var take = 10; var skip = 0;

            for (int i = 0; i < take; i++)
            {
                await CreateOrderApi();
            }

            // Act
            var result = await _httpClient.GetAsync($"/api/v1/Order/{take}/{skip}");
            var receiveStream = await result.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();
            var order = JsonConvert.DeserializeObject<List<Order>>(stringContent) ?? new();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(order.Any());
                Assert.That(order, Has.Count.EqualTo(take));
            });
        }

        private async Task<Order> GetOrderApi(Guid orderId)
        {
            var result = await _httpClient.GetAsync($"/api/v1/Order/{orderId}");
            var receiveStream = await result.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            return JsonConvert.DeserializeObject<Order>(stringContent) ?? new();
        }

        private async Task<Guid> CreateOrderApi(int itemsQty = 1)
        {
            var content = new StringContent(JsonConvert.SerializeObject(
                GetCreateOrderCommandStub(itemsQty)),
                Encoding.UTF8, "application/json");
            var createdOrder = await _httpClient.PostAsync($"/api/v1/Order", content);
            var receiveStream = await createdOrder.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();

            Assert.That(createdOrder.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            return JsonConvert.DeserializeObject<Guid>(stringContent);
        }

        private static CreateOrderCommand GetCreateOrderCommandStub(int itemsQty = 1)
        {
            var command = new CreateOrderCommand()
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
                            ProductId = DbSeed.ProductSeedId,
                            Quantity = 10
                        }
                    }
            };

            if (itemsQty > 1)
            {
                command.OrderItems.Add(new CreateOrderItem
                {
                    ProductId = DbSeed.ProductTwoSeedId,
                    Quantity = 10
                });
            }

            if (itemsQty > 2)
            {
                command.OrderItems.Add(new CreateOrderItem
                {
                    ProductId = DbSeed.ProductThreeSeedId,
                    Quantity = 10
                });
            }

            return command;
        }
    }
}

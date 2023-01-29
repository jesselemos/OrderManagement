using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Domain.Models;
using Newtonsoft.Json;
using System.Text;
using OrderService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Entities;
using OrderService.Application.Commands.CancelOrder;
using OrderService.Application.Commands.UpdateOrderAddress;
using OrderService.Application.Commands.UpdateOrderItems;
using OrderService.Infrastructure.DataSeed;

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
            var productId = DbSeed.ProductSeedId;
            var productTwoId = DbSeed.ProductTwoSeedId;
            var productThreeId = DbSeed.ProductThreeSeedId;
            var productQuantityToOrder = 10;
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(productId);
            var productTwoBeforeOrder = await ProductRepository.GetProductByIdAsync(productTwoId);
            var productThreeBeforeOrder = await ProductRepository.GetProductByIdAsync(productThreeId);
            var orderStub = GetCreateOrderCommandStub(productId, productQuantityToOrder);
            orderStub.OrderItems.Add(new CreateOrderItem { ProductId = productTwoId, Quantity = productQuantityToOrder });
            orderStub.OrderItems.Add(new CreateOrderItem { ProductId = productThreeId, Quantity = productQuantityToOrder });
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");

            // Act
            var result = await _httpClient.PostAsync($"/api/v1/order", content);

            //assert
            var productAfterOrder = await ProductRepository.GetProductByIdAsync(productId);
            var productTwoAfterOrder = await ProductRepository.GetProductByIdAsync(productTwoId);
            var productThreeAfterOrder = await ProductRepository.GetProductByIdAsync(productThreeId);
            Assert.Multiple(() =>
            {
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock - productQuantityToOrder));
                Assert.That(productTwoAfterOrder?.Stock, Is.EqualTo(productTwoBeforeOrder?.Stock - productQuantityToOrder));
                Assert.That(productThreeAfterOrder?.Stock, Is.EqualTo(productThreeBeforeOrder?.Stock - productQuantityToOrder));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            });
        }

        [Test]
        public async Task CancelOrderSetsCanceledStatusAndReturnsStockToTheProducts()
        {
            //Arrange
            var productId = DbSeed.ProductSeedId;
            var productTwoId = DbSeed.ProductTwoSeedId;
            var productQuantityToOrder = 10;
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(productId);
            var productTwoBeforeOrder = await ProductRepository.GetProductByIdAsync(productTwoId);
            var orderStub = GetCreateOrderCommandStub(productId);
            orderStub.OrderItems.Add(new CreateOrderItem { ProductId = productTwoId, Quantity = productQuantityToOrder });
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");
            var createdOrder = await _httpClient.PostAsync($"/api/v1/order", content);
            var receiveStream = await createdOrder.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();
            var createdOrderId = JsonConvert.DeserializeObject<Guid>(stringContent);

            // Act
            var command = new CancelOrderCommand { OrderId = createdOrderId };
            content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var cancelResult = await _httpClient.PutAsync($"/api/v1/order/cancel", content);

            //assert
            var productAfterOrder = await ProductRepository.GetProductByIdAsync(productId);
            var productTwoAfterOrder = await ProductRepository.GetProductByIdAsync(productTwoId);

            var getOrderResult = await _httpClient.GetAsync($"/api/v1/Order/{createdOrderId}");
            receiveStream = await getOrderResult.Content.ReadAsStreamAsync();
            readStream = new StreamReader(receiveStream, Encoding.UTF8);
            stringContent = readStream.ReadToEnd();
            var returnedOrder = JsonConvert.DeserializeObject<Order>(stringContent) ?? new();

            Assert.Multiple(() =>
            {
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock));
                Assert.That(productTwoAfterOrder?.Stock, Is.EqualTo(productTwoBeforeOrder?.Stock));
                Assert.That(cancelResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedOrder.OrderStatus, Is.EqualTo(OrderStatus.Canceled));
            });
        }

        [Test]
        public async Task UpdateOrderAddressSetsTheAddressCorrectly()
        {
            //Arrange
            var orderStub = GetCreateOrderCommandStub(DbSeed.ProductSeedId);
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");
            var createdOrder = await _httpClient.PostAsync($"/api/v1/order", content);
            var receiveStream = await createdOrder.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();
            var createdOrderId = JsonConvert.DeserializeObject<Guid>(stringContent);

            // Act
            var command = new UpdateOrderAddressCommand()
            {
                OrderId = createdOrderId,
                AddressLine = "Apartment 7, New Address Line",
                AddressName = "New Work Address",
                EirCode = "V01HT01",
                County = "New County"
            };
            content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var updateAddressResult = await _httpClient.PutAsync($"/api/v1/order/address", content);

            //assert
            var getOrderResult = await _httpClient.GetAsync($"/api/v1/Order/{createdOrderId}");
            receiveStream = await getOrderResult.Content.ReadAsStreamAsync();
            readStream = new StreamReader(receiveStream, Encoding.UTF8);
            stringContent = readStream.ReadToEnd();
            var returnedOrder = JsonConvert.DeserializeObject<Order>(stringContent) ?? new();

            Assert.Multiple(() =>
            {
                Assert.That(updateAddressResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedOrder.OrderStatus, Is.EqualTo(OrderStatus.Created));
                Assert.That(returnedOrder.AddressLine, Is.EqualTo("Apartment 7, New Address Line"));
                Assert.That(returnedOrder.AddressName, Is.EqualTo("New Work Address"));
                Assert.That(returnedOrder.EirCode, Is.EqualTo("V01HT01"));
                Assert.That(returnedOrder.County, Is.EqualTo("New County"));
            });
        }

        [Test]
        public async Task UpdateOrderItemsSetsTheItemsAndProductStocksCorrectly()
        {
            //Arrange
            var productId = DbSeed.ProductSeedId;
            var productTwoId = DbSeed.ProductTwoSeedId;
            var productThreeId = DbSeed.ProductThreeSeedId;
            var productQuantityToOrder = 10;
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(productId);
            var productTwoBeforeOrder = await ProductRepository.GetProductByIdAsync(productTwoId);
            var productThreeBeforeOrder = await ProductRepository.GetProductByIdAsync(productThreeId);
            var orderStub = GetCreateOrderCommandStub(DbSeed.ProductSeedId);
            orderStub.OrderItems.Add(new CreateOrderItem { ProductId = DbSeed.ProductTwoSeedId, Quantity = productQuantityToOrder });
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");
            var createdOrder = await _httpClient.PostAsync($"/api/v1/order", content);
            var receiveStream = await createdOrder.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();
            var createdOrderId = JsonConvert.DeserializeObject<Guid>(stringContent);

            var productTwoNewQuantity = 2;
            // Act
            var command = new UpdateOrderItemsCommand()
            {
                OrderId = createdOrderId,
                OrderItems = new List<CreateOrderItem>
                {
                    //productOne left out on purpose
                    new CreateOrderItem
                    {
                        ProductId = productTwoId,
                        Quantity = productTwoNewQuantity
                    },
                    //new product
                    new CreateOrderItem
                    {
                        ProductId = productThreeId,
                        Quantity = productQuantityToOrder
                    },
                }
            };
            content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            var updateItemsResult = await _httpClient.PutAsync($"/api/v1/order/items", content);

            //asserting product stock
            var productAfterOrder = await ProductRepository.GetProductByIdAsync(productId);
            var productTwoAfterOrder = await ProductRepository.GetProductByIdAsync(productTwoId);
            var productThreeAfterOrder = await ProductRepository.GetProductByIdAsync(productThreeId);
            Assert.Multiple(() =>
            {
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock));
                Assert.That(productTwoAfterOrder?.Stock, Is.EqualTo(productTwoBeforeOrder?.Stock - productTwoNewQuantity));
                Assert.That(productThreeAfterOrder?.Stock, Is.EqualTo(productThreeBeforeOrder?.Stock - productQuantityToOrder));
                Assert.That(updateItemsResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

            //asserting new orderItens are as expected
            var getOrderResult = await _httpClient.GetAsync($"/api/v1/Order/{createdOrderId}");
            receiveStream = await getOrderResult.Content.ReadAsStreamAsync();
            readStream = new StreamReader(receiveStream, Encoding.UTF8);
            stringContent = readStream.ReadToEnd();
            var returnedOrder = JsonConvert.DeserializeObject<Order>(stringContent) ?? new();

            Assert.Multiple(() =>
            {
                Assert.That(updateItemsResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedOrder.OrderItems, Has.Count.EqualTo(2));
                Assert.That(!returnedOrder.OrderItems.Any(f => f.Product.Id == productId));
                Assert.That(returnedOrder.OrderItems.Single(f => f.Product.Id == productTwoId).Quantity, Is.EqualTo(productTwoNewQuantity));
                Assert.That(returnedOrder.OrderItems.Single(f => f.Product.Id == productThreeId).Quantity, Is.EqualTo(productQuantityToOrder));
            });
        }

        [Test]
        public async Task CreateOrderWithInvalidRequestReturnsValidationErrors()
        {
            //Arrange
            var orderStub = new CreateOrderCommand();
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");

            // Act
            var result = await _httpClient.PostAsync($"/api/v1/order", content);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
            });
        }

        [Test]
        public async Task GetOrderByIdReturnsCreatedOrder()
        {
            //Arrange
            var orderStub = GetCreateOrderCommandStub(DbSeed.ProductSeedId);
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");
            var createdOrder = await _httpClient.PostAsync($"/api/v1/Order", content);
            var receiveStream = await createdOrder.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();
            var createdOrderId = JsonConvert.DeserializeObject<Guid>(stringContent);

            // Act
            var result = await _httpClient.GetAsync($"/api/v1/Order/{createdOrderId}");
            receiveStream = await result.Content.ReadAsStreamAsync();
            readStream = new StreamReader(receiveStream, Encoding.UTF8);
            stringContent = readStream.ReadToEnd();
            var returnedOrder = JsonConvert.DeserializeObject<Order>(stringContent) ?? new();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(createdOrder.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedOrder.OrderStatus, Is.EqualTo(OrderStatus.Created));
                Assert.That(returnedOrder.Total, Is.EqualTo(60));
            });
        }

        [Test]
        public async Task GetOrdersReturnsPaginatedOrders()
        {
            //Arrange
            var take = 10;
            var skip = 0;
            var orderStub = GetCreateOrderCommandStub(DbSeed.ProductSeedId);
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");
            var createdOrder = await _httpClient.PostAsync($"/api/v1/Order", content);

            // Act
            var result = await _httpClient.GetAsync($"/api/v1/Order/{take}/{skip}");
            var receiveStream = await result.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var stringContent = readStream.ReadToEnd();
            var returnedOrder = JsonConvert.DeserializeObject<List<Order>>(stringContent) ?? new();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(createdOrder.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedOrder.Any());
            });
        }

        private static CreateOrderCommand GetCreateOrderCommandStub(Guid productId, int quantity = 10)
        {
            return new CreateOrderCommand()
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
                            Quantity = quantity
                        }
                    }
            };
        }
    }
}

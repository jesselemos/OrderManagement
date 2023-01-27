using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using OrderService.API.Tests.UnitTests.Helpers;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Models;
using Newtonsoft.Json;
using System.Text;
using OrderService.API.Repositories;
using Microsoft.Extensions.DependencyInjection;
using OrderService.API.Entities;
using OrderService.API.Commands.CancelOrder;
using OrderService.API.Commands.UpdateOrderAddress;

namespace OrderService.API.Tests.IntegrationTests
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
                return new ProductRepository(context ?? DatabaseHelper.GetOrderDbContext());
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
            var productId = DatabaseHelper.ProductSeedId;
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(productId);
            var productQuantityToOrder = 3;
            var orderStub = GetCreateOrderCommandStub(productId, productQuantityToOrder);
            var content = new StringContent(JsonConvert.SerializeObject(orderStub), Encoding.UTF8, "application/json");

            // Act
            var result = await _httpClient.PostAsync($"/api/v1/order", content);

            //assert
            var productAfterOrder = await ProductRepository.GetProductByIdAsync(productId);
            Assert.Multiple(() =>
            {
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock - productQuantityToOrder));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            });
        }

        [Test]
        public async Task CancelOrderSetsCanceledStatusAndReturnsStockToTheProducts()
        {
            //Arrange
            var productId = DatabaseHelper.ProductSeedId;
            var productBeforeOrder = await ProductRepository.GetProductByIdAsync(productId);
            var orderStub = GetCreateOrderCommandStub(productId);
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

            var getOrderResult = await _httpClient.GetAsync($"/api/v1/Order/{createdOrderId}");
            receiveStream = await getOrderResult.Content.ReadAsStreamAsync();
            readStream = new StreamReader(receiveStream, Encoding.UTF8);
            stringContent = readStream.ReadToEnd();
            var returnedOrder = JsonConvert.DeserializeObject<Order>(stringContent);

            Assert.Multiple(() =>
            {
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock));
                Assert.That(cancelResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedOrder.OrderStatus, Is.EqualTo(OrderStatus.Canceled));
            });
        }

        [Test]
        public async Task UpdateOrderAddressSetsTheInformedAddressCorrectly()
        {
            //Arrange
            var orderStub = GetCreateOrderCommandStub(DatabaseHelper.ProductSeedId);
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
            var returnedOrder = JsonConvert.DeserializeObject<Order>(stringContent);

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
        public async Task GetOrderByIdReturnsCreatedOrder()
        {
            //Arrange
            var orderStub = GetCreateOrderCommandStub(DatabaseHelper.ProductSeedId);
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
            var returnedOrder = JsonConvert.DeserializeObject<Order>(stringContent);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(createdOrder.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedOrder.OrderStatus, Is.EqualTo(OrderStatus.Created));
                Assert.That(returnedOrder.Total, Is.EqualTo(6));
            });
        }

        private CreateOrderCommand GetCreateOrderCommandStub(Guid productId, int quantity = 1)
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

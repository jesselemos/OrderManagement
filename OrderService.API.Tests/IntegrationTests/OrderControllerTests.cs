using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using OrderService.API.Tests.UnitTests.Helpers;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Models;
using Newtonsoft.Json;
using System.Text;
using OrderService.API.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.API.Tests.IntegrationTests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;
        private IProductRepository _productRepository
        {
            get
            {
                var scopeFactory = _factory.Services;
                var scope = scopeFactory.CreateScope();
                return new ProductRepository(scope.ServiceProvider.GetService<OrderDbContext>());
            }
        }

        [SetUp]
        public void Init()
        {
            _factory = new WebApplicationFactory<Program>();
            _httpClient = _factory.CreateClient();
        }

        [Test]
        public async Task CreateOrderDecreasesProductStock()
        {
            //Arrange
            var productId = DatabaseHelper.ProductSeedId;
            var productBeforeOrder = await _productRepository.GetProductByIdAsync(productId);
            var productQuantityToOrder = 3;
            var content = new StringContent(JsonConvert.SerializeObject(
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
                            Quantity = productQuantityToOrder
                        }
                    }
            }), Encoding.UTF8, "application/json");

            // Act
            var result = await _httpClient.PostAsync($"/api/v1/Order", content);

            //assert
            var productAfterOrder = await _productRepository.GetProductByIdAsync(productId);
            Assert.Multiple(() =>
            {
                Assert.That(productAfterOrder?.Stock, Is.EqualTo(productBeforeOrder?.Stock - productQuantityToOrder));
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            });
        }

        //[Test]
        //public async Task GetOrderByIdReturnsOrder()
        //{
        //    // Act
        //    var result = await _httpClient.GetAsync($"/api/v1/Order/{DatabaseHelper.OrderSeedId}");

        //    //assert
        //    Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        //}
    }
}

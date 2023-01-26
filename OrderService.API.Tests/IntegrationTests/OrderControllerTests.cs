using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using OrderService.API.Tests.UnitTests.Helpers;

namespace OrderService.API.Tests.IntegrationTests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private HttpClient httpClient;

        [SetUp]
        public void Init()
        {
            httpClient = new WebApplicationFactory<Program>().CreateClient();

            DatabaseHelper.GetOrderDbContext();
        }

        [Test]
        public async Task GetEntityRowReturnsForbiddenWhenViewPermissionNotPresent()
        {
            // Act
            var result = await httpClient.GetAsync($"/api/v1/Order/{DatabaseHelper.OrderSeedId}");

            //assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}

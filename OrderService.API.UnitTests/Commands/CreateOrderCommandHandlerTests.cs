using Microsoft.EntityFrameworkCore;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.UnitTests
{
    public class CreateOrderCommandHandlerTests
    {
        public DbContextOptions<OrderDbContext> _orderDbContextOptions;
        private OrderDbContext _orderDbContext;

        [SetUp]
        public void Setup()
        {
            _orderDbContextOptions = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .EnableSensitiveDataLogging()
                .Options;

            _orderDbContext = new OrderDbContext(_orderDbContextOptions);
        }

        [Test]
        public void Test1()
        {
            var repository = new OrderRepository(_orderDbContext);

            var obj = new CreateOrderCommandHandler(repository).Handle(
                new CreateOrderCommand(
                new Order
                {
                    // TotalPrice = 10
                }), new CancellationToken());

            Assert.That(obj, Is.Not.Null);
            Assert.Pass();
        }
    }
}
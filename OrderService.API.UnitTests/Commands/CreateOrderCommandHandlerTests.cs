using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Repositories;

namespace OrderService.API.UnitTests
{
    public class CreateOrderCommandHandlerTests
    {
        public DbContextOptions<OrderDbContext> _orderDbContextOptions;
        private OrderDbContext _orderDbContext;
        private readonly IMapper? _mapper;

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

            var obj = new CreateOrderCommandHandler(repository, _mapper).Handle(
                new CreateOrderCommand()
                {
                    CustomerName = "Name",
                    //TotalPrice = 10,
                    //ProductId = Guid.NewGuid()
                }, new CancellationToken());

            Assert.That(obj, Is.Not.Null);
            Assert.Pass();
        }
    }
}
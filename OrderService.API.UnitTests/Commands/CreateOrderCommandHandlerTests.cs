using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Repositories;
using OrderService.API.UnitTests.Helpers;

namespace OrderService.API.UnitTests
{
    public class CreateOrderCommandHandlerTests
    {
        public DbContextOptions<OrderDbContext> _orderDbContextOptions;
        private OrderDbContext _orderDbContext;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _orderDbContextOptions = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .EnableSensitiveDataLogging()
                .Options;

            _orderDbContext = new OrderDbContext(_orderDbContextOptions);

            _mapper = AutoMapperHelper.CreateMapper();
        }

        [Test]
        public void Test1()
        {
            var orderRepository = new OrderRepository(_orderDbContext);
            var productRepository = new ProductRepository(_orderDbContext);

            var obj = new CreateOrderCommandHandler(orderRepository, productRepository, _mapper).Handle(
                new CreateOrderCommand()
                {
                    CustomerName = "Name",
                    AddressLine = "AddressLine",
                    AddressName = "AddressName",
                    EirCode = "EirCode",
                    County = "County",
                    OrderItems = new List<Models.CreateOrderItem>
                    {
                        new Models.CreateOrderItem
                        {
                            ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                            Quantity = 10
                        }
                    }

                    //TotalPrice = 10,
                    //ProductId = Guid.NewGuid()
                }, new CancellationToken());

            Assert.That(obj, Is.Not.Null);
            Assert.Pass();
        }
    }
}
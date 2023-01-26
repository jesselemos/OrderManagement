using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Entities;
using OrderService.API.Models;
using OrderService.API.Repositories;
using OrderService.API.UnitTests.Helpers;

namespace OrderService.API.UnitTests
{
    [TestFixture]
    public class CreateOrderCommandHandlerTests
    {
        public DbContextOptions<OrderDbContext> _orderDbContextOptions;
        private OrderDbContext _orderDbContext;
        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _orderDbContextOptions = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .EnableSensitiveDataLogging()
                .Options;

            _orderDbContext = new OrderDbContext(_orderDbContextOptions);

            _orderDbContext.Database.EnsureDeleted();
            _orderDbContext.Database.EnsureCreated();

            _orderDbContext.Products.Add(new Product { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Name = "Brownie", Price = 6, Stock = 10 });

            _orderDbContext.SaveChangesAsync();

            _mapper = AutoMapperHelper.CreateMapper();
            _orderRepository = new OrderRepository(_orderDbContext);
            _productRepository = new ProductRepository(_orderDbContext);
        }

        [Test]
        public async Task CanCreateOrder()
        {
            var result = await new CreateOrderCommandHandler(_orderRepository, _productRepository, _mapper).Handle(
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
                            ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                            Quantity = 10
                        }
                    }
                }, new CancellationToken());
            Assert.That(Guid.TryParse(result.ToString(), out _), Is.True);
        }

        [Test]
        public void ThrowExceptionIfProductNotExistsInDatabase()
        {
            var commandHandler = new CreateOrderCommandHandler(_orderRepository, Substitute.For<IProductRepository>(), _mapper);
            Guid productId = new();

            Assert.That(async () => 
                await commandHandler.Handle(
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
                    }, new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"ProductId: {productId} not found in our database"));
        }

        [Test]
        public async Task ThrowExceptionIfThereIsNotEnoughStockForTheProductAsync()
        {
            var commandHandler = new CreateOrderCommandHandler(_orderRepository, _productRepository, _mapper);
            var productId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var product = await _productRepository.GetProductByIdAsync(productId);

            Assert.That(async () =>
                    await commandHandler.Handle(
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
                        }, new CancellationToken()),
                        Throws.TypeOf<Exception>()
                        .With.Message.EqualTo($"There is only {product?.Stock} units available of {product?.Name}"));
        }
    }
}
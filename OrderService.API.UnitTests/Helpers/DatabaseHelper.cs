using Microsoft.EntityFrameworkCore;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.UnitTests.Helpers
{
    public static class DatabaseHelper
    {
        public static Guid ProductSeedId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        public static Guid OrderSeedId = Guid.Parse("76ba2048-edfd-4ed5-a8a4-fe8ae40e492f");
        public static OrderDbContext GetOrderDbContext()
        {
            var orderDbContextOptions = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .EnableSensitiveDataLogging()
            .Options;

            var orderDbContext = new OrderDbContext(orderDbContextOptions);
            orderDbContext.Database.EnsureDeleted();
            orderDbContext.Database.EnsureCreated();

            orderDbContext.Products.Add(new Product { Id = ProductSeedId, Name = "Brownie", Price = 6, Stock = 100 });
            orderDbContext.SaveChangesAsync();


            orderDbContext.Orders.Add(new Order()
            {
                Id = OrderSeedId,
                CustomerName = "Name",
                AddressLine = "AddressLine",
                AddressName = "AddressName",
                EirCode = "EirCode",
                County = "County",
                OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                Product = orderDbContext.Products.First(),
                                Quantity = 10,
                            }
                        }
            });
            orderDbContext.SaveChangesAsync();

            for (var i = 0; i < 100; i++)
            {
                orderDbContext.Orders.Add(new Order()
                {
                    CustomerName = "Name",
                    AddressLine = "AddressLine",
                    AddressName = "AddressName",
                    EirCode = "EirCode",
                    County = "County",
                    OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                Product = new Product
                                {
                                    Name= "Name",
                                    Price = 10,
                                },
                                Quantity = 10,
                            }
                        }
                });
                orderDbContext.SaveChangesAsync();
            }

            orderDbContext.SaveChangesAsync();

            return orderDbContext;
        }
    }
}

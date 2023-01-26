using Microsoft.EntityFrameworkCore;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.UnitTests.Helpers
{
    public static class DatabaseHelper
    {
        public static OrderDbContext GetOrderDbContext()
        {
            var orderDbContextOptions = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .EnableSensitiveDataLogging()
            .Options;

            var orderDbContext = new OrderDbContext(orderDbContextOptions);
            orderDbContext.Database.EnsureDeleted();
            orderDbContext.Database.EnsureCreated();

            orderDbContext.Products.Add(new Product { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Name = "Brownie", Price = 6, Stock = 10 });

            orderDbContext.SaveChangesAsync();

            return orderDbContext;
        }
    }
}

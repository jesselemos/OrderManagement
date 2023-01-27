using OrderService.API.Entities;

namespace OrderService.API.Repositories.DataSeed
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void SeedData()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            using var orderDbContext = serviceScope.ServiceProvider.GetService<OrderDbContext>();
            if (orderDbContext != null && !orderDbContext.Products.Any())
            {
                orderDbContext.Products.Add(new Product { Name = "Coffee", Price = 4, Stock = 1000 });
                orderDbContext.Products.Add(new Product { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Name = "Brownie", Price = 6, Stock = 1000 });
                orderDbContext.Products.Add(new Product { Id = Guid.Parse("835c7288-b80d-4e61-9f39-cd8bc58dd668"), Name = "Water", Price = 2, Stock = 2000 });
                orderDbContext.Products.Add(new Product { Id = Guid.Parse("8f6a6044-7ac0-4d17-8148-f0f520d571e5"), Name = "Orange Juice", Price = 3, Stock = 3000 });

                orderDbContext.SaveChangesAsync();
            }
        }
    }
}

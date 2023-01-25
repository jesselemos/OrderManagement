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
            using var context = serviceScope.ServiceProvider.GetService<OrderDbContext>();
            if (!context.Products.Any())
            {
                context.Products.Add(new Product { Name = "Coffee", Price = 4, Stock = 10 });
                context.Products.Add(new Product { Name = "Water", Price = 2, Stock = 10 });
                context.Products.Add(new Product { Name = "Orange Juice", Price = 3, Stock = 10 });
                context.Products.Add(new Product { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Name = "Brownie", Price = 6, Stock = 10 });
            }

            context.SaveChangesAsync();
        }
    }
}

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
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<OrderDbContext>())
                {
                     if (!context.Products.Any())
                    {
                        var prod = new Product { Name = "Cofee", Price = 4, Stock = 10 };
                        context.Products.Add(prod);
                    }

                    context.SaveChangesAsync();
                }
            }
        }
    }
}

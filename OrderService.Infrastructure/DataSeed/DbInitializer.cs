using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.DataSeed
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task SeedData()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            using var orderDbContext = serviceScope.ServiceProvider.GetService<OrderDbContext>();
            await DbSeed.AddSeedProducts(orderDbContext);
        }
    }
}

namespace OrderService.Infrastructure.DataSeed
{
    public interface IDbInitializer
    {
        Task SeedData();
    }
}

namespace OrderService.Domain.Interfaces
{
    public interface IDbInitializer
    {
        Task SeedData();
    }
}

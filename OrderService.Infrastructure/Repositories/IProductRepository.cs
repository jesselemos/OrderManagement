using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        public Task UpdateProductAsync(Product product);
        public Task<Product?> GetProductByIdAsync(Guid id);
    }
}

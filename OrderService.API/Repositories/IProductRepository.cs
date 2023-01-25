using OrderService.API.Entities;

namespace OrderService.API.Repositories
{
    public interface IProductRepository
    {
        public Task UpdateProductAsync(Product product);
        public Task<Product?> GetProductByIdAsync(Guid id);
    }
}

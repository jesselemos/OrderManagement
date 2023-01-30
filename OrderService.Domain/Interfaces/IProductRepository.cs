using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces
{
    public interface IProductRepository
    {
        public Task UpdateProductAsync(Product product);
        public Task<Product?> GetProductByIdAsync(Guid id);
    }
}

using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderDbContext _orderDbContext;
        public ProductRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }
        public async Task UpdateProductAsync(Product product)
        {
            _orderDbContext.Entry(product).State = EntityState.Modified;
            await _orderDbContext.SaveChangesAsync();
        }
        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _orderDbContext.Products.SingleOrDefaultAsync(s => s.Id == id);
        }
    }
}

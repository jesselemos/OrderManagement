using MediatR;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderNotificationHandler : INotificationHandler<CreateOrderNotification>
    {
        private readonly IProductRepository _productRepository;

        public CreateOrderNotificationHandler(IProductRepository productRepository) => _productRepository = productRepository;

        public async Task Handle(CreateOrderNotification notification, CancellationToken cancellationToken)
        {
            foreach (var item in notification.Order.OrderItems)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new ArgumentNullException($"ProductId: {item.ProductId} not found in our database.");
                }

                if (product.Stock < item.Quantity)
                {
                    throw new ArgumentOutOfRangeException($"There is only {product.Stock} units available of {product.Name}");
                }

                product.Stock -= item.Quantity;

                await _productRepository.UpdateProductAsync(product);
            }
            await Task.CompletedTask;
        }
    }
}

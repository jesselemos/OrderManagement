using MediatR;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CancelOrder
{
    public class CancelOrderNotificationHandler : INotificationHandler<CancelOrderNotification>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public CancelOrderNotificationHandler(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task Handle(CancelOrderNotification notification, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(notification.Order.OrderId);
            if (order == null)
            {
                throw new Exception($"OrderId: {notification.Order.OrderId} not found in our database.");
            }

            foreach (var item in order.OrderItems)
            {
                //todo cover condition with unit tests
                var product = await _productRepository.GetProductByIdAsync(item.Product.Id);
                if (product == null)
                {
                    continue;
                }

                product.Stock += item.Quantity;

                await _productRepository.UpdateProductAsync(product);
            }
            await Task.CompletedTask;
        }
    }
}

using MediatR;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsNotificationHandler : INotificationHandler<UpdateOrderItemsNotification>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public UpdateOrderItemsNotificationHandler(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task Handle(UpdateOrderItemsNotification notification, CancellationToken cancellationToken)
        {
            var order = notification.Order.PreviousOrder;
            if (order == null)
            {
                order = await _orderRepository.GetOrderByIdAsync(notification.Order.OrderId);
                if (order == null)
                {
                    throw new Exception($"OrderId: {notification.Order.OrderId} not found in our database.");
                }
            }

            //returning product stock for removed items
            foreach (var item in order.OrderItems)
            {
                if (!notification.Order.OrderItems.Any(c => c.ProductId == item.Product?.Id))
                {
                    var product = await _productRepository.GetProductByIdAsync(item.Product.Id);
                    if (product == null)
                    {
                        continue;
                    }

                    product.Stock += item.Quantity;

                    await _productRepository.UpdateProductAsync(product);
                }
            }

            //updating product stock for changed and new items
            foreach (var item in notification.Order.OrderItems)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    continue;
                }

                var oldStock = order.OrderItems.SingleOrDefault(c => c.Product.Id == item.ProductId)?.Quantity;

                //Item Changed
                if (oldStock.HasValue)
                {
                    product.Stock += oldStock.Value - item.Quantity;
                }
                else //New Item Added
                {
                    product.Stock -= item.Quantity;
                }

                await _productRepository.UpdateProductAsync(product);
            }

            await Task.CompletedTask;
        }
    }
}

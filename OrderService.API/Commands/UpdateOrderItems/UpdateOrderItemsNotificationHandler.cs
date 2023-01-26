using MediatR;
using OrderService.API.Entities;
using OrderService.API.Exceptions;
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
            var order = await _orderRepository.GetOrderByIdAsync(notification.Order.OrderId);
            if (order == null)
            {
                throw new NotFoundException(nameof(Order), notification.Order.OrderId);
            }

            //returning product stock for removed items
            foreach (var item in order.OrderItems)
            {
                if (!notification.Order.OrderItems.Any(c => c.ProductId == item.Product?.Id))
                {
                    var product = await _productRepository.GetProductByIdAsync(item.Product.Id);
                    if (product == null)
                    {
                        throw new Exception($"ProductId: {item.Product.Id} not found in our database");
                    }

                    product.Stock += item.Quantity;

                    await _productRepository.UpdateProductAsync(product);
                }
            }

            //decreasing product stock for existing items
            foreach (var item in notification.Order.OrderItems)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"ProductId: {item.ProductId} not found in our database");
                }

                var oldStock = order.OrderItems.Single(c => c.Product.Id == item.ProductId).Quantity;

                product.Stock += oldStock - item.Quantity;

                await _productRepository.UpdateProductAsync(product);
            }

            await Task.CompletedTask;
        }
    }
}

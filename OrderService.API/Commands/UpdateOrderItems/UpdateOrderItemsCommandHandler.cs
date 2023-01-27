using MediatR;
using OrderService.API.Entities;
using OrderService.API.Extensions;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsCommandHandler : IRequestHandler<UpdateOrderItemsCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public UpdateOrderItemsCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Order> Handle(UpdateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                throw new ArgumentNullException($"OrderId: {request.OrderId} not found in our database.");
            }

            var orderBeforeUpdate = order.DeepCopy();

            List<OrderItem> newItems = new();

            foreach (var item in request.OrderItems)
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

                newItems.Add(new OrderItem
                {
                    Product = product,
                    Quantity = item.Quantity
                });
            }

            await _orderRepository.UpdateOrderItemsAsync(order, newItems);

            return orderBeforeUpdate;
        }
    }
}

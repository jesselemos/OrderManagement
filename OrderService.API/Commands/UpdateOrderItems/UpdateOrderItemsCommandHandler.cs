using MediatR;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsCommandHandler : IRequestHandler<UpdateOrderItemsCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public UpdateOrderItemsCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(UpdateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                throw new Exception($"OrderId: {request.OrderId} not found in our database.");
            }

            List<OrderItem> newItems = new();

            foreach (var item in request.OrderItems)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"ProductId: {item.ProductId} not found in our database.");
                }

                if (product.Stock < item.Quantity)
                {
                    throw new Exception($"There is only {product.Stock} units available of {product.Name}");
                }

                newItems.Add(new OrderItem
                {
                    Product = product,
                    Quantity = item.Quantity
                });
            }

            await _orderRepository.UpdateOrderItemsAsync(order, newItems);

            return Unit.Value;
        }
    }
}

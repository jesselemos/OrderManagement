using AutoMapper;
using MediatR;
using OrderService.API.Entities;
using OrderService.API.Exceptions;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsCommandHandler : IRequestHandler<UpdateOrderItemsCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public UpdateOrderItemsCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (orderToUpdate == null)
            {
                throw new NotFoundException(nameof(Order), request.OrderId);
            }

            List<OrderItem> newItems = new();

            foreach (var item in request.OrderItems)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"ProductId: {item.ProductId} not found in our database");
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

            await _orderRepository.UpdateOrderItemsAsync(orderToUpdate, newItems);

            return Unit.Value;
        }
    }
}

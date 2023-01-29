using AutoMapper;
using MediatR;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = _mapper.Map<Order>(request);
            newOrder.OrderItems = new();

            foreach (var item in request.OrderItems)
            {
                if (item.Quantity < 1)
                {
                    throw new NotSupportedException($"Quantity: {item.Quantity} should be greater than 0.");
                }

                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new ArgumentNullException($"ProductId: {item.ProductId} not found in our database.");
                }

                if (product.Stock < item.Quantity)
                {
                    throw new ArgumentOutOfRangeException($"There is only {product.Stock} units available of {product.Name}");
                }

                newOrder.OrderItems.Add(new OrderItem
                {
                    Product = product,
                    Quantity = item.Quantity
                });
            }

            await _orderRepository.CreateOrderAsync(newOrder);
            return newOrder.Id;
        }
    }
}

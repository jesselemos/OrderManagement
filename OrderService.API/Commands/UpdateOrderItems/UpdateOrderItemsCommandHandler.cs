using AutoMapper;
using MediatR;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class UpdateOrderItemsCommandHandler : IRequestHandler<UpdateOrderItemsCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public UpdateOrderItemsCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetOrderByIdAsync(request.OrderId);

            //TODO::Update Items

            _mapper.Map(request, orderToUpdate, typeof(UpdateOrderItemsCommand), typeof(Order));


            await _orderRepository.UpdateOrderAsync(orderToUpdate);

            return Unit.Value;
        }
    }
}

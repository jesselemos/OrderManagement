using AutoMapper;
using MediatR;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class UpdateOrderAddressCommandHandler : IRequestHandler<UpdateOrderAddressCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public UpdateOrderAddressCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateOrderAddressCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            //if (orderToUpdate == null)
            //{
            //    throw new NotFoundException(nameof(Order), request.OrderId);
            //}

            _mapper.Map(request, orderToUpdate, typeof(UpdateOrderAddressCommand), typeof(Order));


            await _orderRepository.UpdateOrderAsync(orderToUpdate);

            return Unit.Value;
        }
    }
}

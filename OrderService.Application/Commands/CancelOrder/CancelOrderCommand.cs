using MediatR;

namespace OrderService.Application.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest
    {
        public Guid OrderId { get; set; }
    }
}

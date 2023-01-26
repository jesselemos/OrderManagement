using MediatR;

namespace OrderService.API.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest
    {
        public Guid OrderId { get; set; }
    }
}

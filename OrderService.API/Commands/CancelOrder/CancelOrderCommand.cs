using MediatR;

namespace OrderService.API.Commands.CreateOrder
{
    public class CancelOrderCommand : IRequest
    {
        public Guid OrderId { get; set; }
    }
}

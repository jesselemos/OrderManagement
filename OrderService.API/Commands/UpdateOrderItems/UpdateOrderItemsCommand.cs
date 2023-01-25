using MediatR;

namespace OrderService.API.Commands.CreateOrder
{
    public class UpdateOrderItemsCommand : IRequest
    {
        public Guid OrderId { get; set; }
        public Guid? ProductId { get; set; }
    }
}

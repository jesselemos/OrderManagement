using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public string? CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        //TODO::Map new entities to datatable
        public Guid? ProductId { get; set; }
        // public Address? DeliveryAddress { get; set; }
    }
}

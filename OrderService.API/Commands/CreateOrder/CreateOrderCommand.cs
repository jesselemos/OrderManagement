using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public string? CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        //TODO::Map new entities to datatable
        public Guid? ProductId { get; set; }
        public string? AddressName { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
    }
}

using AutoMapper;
using OrderService.API.Commands.CancelOrder;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Commands.UpdateOrderAddress;
using OrderService.API.Commands.UpdateOrderItems;
using OrderService.API.Entities;
using OrderService.API.Models;

namespace OrderService.API.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<CreateOrderCommand, Order>();
            CreateMap<CreateOrderItem, OrderItem>();

            CreateMap<UpdateOrderAddressCommand, Order>();
            CreateMap<UpdateOrderItemsCommand, Order>();
            CreateMap<CancelOrderCommand, Order>();
        }
    }
}

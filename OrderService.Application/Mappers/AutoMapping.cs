using AutoMapper;
using OrderService.Application.Commands.CancelOrder;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Commands.UpdateOrderAddress;
using OrderService.Application.Commands.UpdateOrderItems;
using OrderService.Domain.Entities;
using OrderService.Domain.Models;

namespace OrderService.Application.Mappers
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

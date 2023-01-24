using MediatR;
using OrderService.API.Commands.CreateOrder;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using OrderService.API.Entities;
using OrderService.API.Queries.GetOrders;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost(Name = "CreateOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CreateOrder([FromBody] Order order)
        {
            var createdOrder = await _mediator.Send(new CreateOrderCommand(order));

            await _mediator.Publish(new CreateOrderNotification(createdOrder));

            return CreatedAtRoute("GetOrderById", new { id = createdOrder.Id }, createdOrder);
        }

        [HttpGet("{id:Guid}", Name = "GetOrderById")]
        public async Task<ActionResult> GetOrderById(Guid id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery(id));
            return Ok(order);
        }


        [HttpGet(Name = "Order")]
        public async Task<ActionResult> GetOrders()
        {
            var orders = await _mediator.Send(new GetOrdersQuery());
            return Ok(orders);
        }
    }
}
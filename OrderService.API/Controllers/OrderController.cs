using MediatR;
using OrderService.API.Commands.CreateOrder;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using OrderService.API.Queries.GetOrders;
using OrderService.API.Entities;
using OrderService.API.Commands.UpdateOrderItems;
using OrderService.API.Commands.CancelOrder;
using OrderService.API.Commands.UpdateOrderAddress;
using OrderService.API.Queries.GetOrderById;

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
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var createdOrder = await _mediator.Send(command);

            await _mediator.Publish(new CreateOrderNotification(command));

            return CreatedAtRoute("GetOrderById", new { id = createdOrder }, createdOrder);
        }

        [HttpPut("address", Name = "UpdateOrderAddress")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateOrderAddress([FromBody] UpdateOrderAddressCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPut("items", Name = "UpdateOrderItems")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateOrderItems([FromBody] UpdateOrderItemsCommand command)
        {
            await _mediator.Send(command);

            await _mediator.Publish(new UpdateOrderItemsNotification(command));

            return Ok();
        }

        [HttpDelete(Name = "CancelOrder")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderCommand command)
        {
            await _mediator.Send(command);

            await _mediator.Publish(new CancelOrderNotification(command));

            return Ok();
        }

        [HttpGet("{id:Guid}", Name = "GetOrderById")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetOrderById(Guid id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery(id));
            return Ok(order);
        }

        [HttpGet("{take:int}/{skip:int}", Name = "Order")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetOrders(int take = 10, int skip = 0)
        {
            var orders = await _mediator.Send(new GetOrdersQuery(take, skip));
            return Ok(orders);
        }
    }
}
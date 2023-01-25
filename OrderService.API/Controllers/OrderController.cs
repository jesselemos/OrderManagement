using MediatR;
using OrderService.API.Commands.CreateOrder;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using OrderService.API.Queries.GetOrders;
using OrderService.API.Entities;

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

        // - Create a new order
        [HttpPost(Name = "CreateOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CreateOrder([FromBody] CreateOrderCommand createOrderCommand)
        {
            var createdOrder = await _mediator.Send(createOrderCommand);

            await _mediator.Publish(new CreateOrderNotification(createOrderCommand));

            //TODO::Investigate Id
            return CreatedAtRoute("GetOrderById", new { id = createdOrder }, createdOrder);
        }

        //- Update the order delivery address
        [HttpPut(Name = "UpdateOrderAddress")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderAddressCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }


        //TODO:: - Update the order items
        //TODO:: - Cancel an order


        //- Retrieve a single order
        [HttpGet("{id:Guid}", Name = "GetOrderById")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetOrderById(Guid id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery(id));
            return Ok(order);
        }


        //TODO:: - Paginate this list of orders
        [HttpGet(Name = "Order")]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetOrders()
        {
            var orders = await _mediator.Send(new GetOrdersQuery());
            return Ok(orders);
        }
    }
}
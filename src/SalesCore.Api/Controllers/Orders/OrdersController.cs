using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesCore.Application.Orders.CreateOrder;
using SalesCore.Application.Orders.DeleteOrder;
using SalesCore.Application.Orders.GetOrderById;
using SalesCore.Application.Orders.UpdateOrder;

namespace SalesCore.Api.Controllers.Orders;

[ApiController]
public class OrdersController(ISender sender) : ControllerBase
{
    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderByIdQuery(orderId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest orderRequest, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateOrderCommand(orderRequest), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPut("{orderId:guid}")]
    public async Task<IActionResult> UpdateOrder([FromRoute] Guid orderId, [FromBody] UpdateOrderRequest orderRequest, CancellationToken cancellationToken)
    {
        if (orderRequest.OrderId != orderId)
        {
            return Forbid();
        }

        var result = await sender.Send(new UpdateOrderCommand(orderRequest), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpDelete("{orderId:guid}")]
    public async Task<IActionResult> DeleteOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteOrderCommand(orderId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }
}
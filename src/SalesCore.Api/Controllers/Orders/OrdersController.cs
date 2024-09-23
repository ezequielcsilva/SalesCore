using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesCore.Application.Orders.CreateOrder;
using SalesCore.Application.Orders.DeleteOrder;
using SalesCore.Application.Orders.GetOrderById;
using SalesCore.Application.Orders.UpdateOrder;
using SalesCore.Domain.Abstractions;

namespace SalesCore.Api.Controllers.Orders;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController(ISender sender) : ControllerBase
{
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(GetOrderByIdResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(typeof(CreateOrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest orderRequest, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateOrderCommand(orderRequest), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpPut("{orderId:guid}")]
    [ProducesResponseType(typeof(UpdateOrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status400BadRequest)]
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

        return Ok(result.Value);
    }

    [HttpDelete("{orderId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(List<Error[]>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteOrderCommand(orderId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}
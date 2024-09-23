using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesCore.Application.Orders.GetOrderById;

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
}
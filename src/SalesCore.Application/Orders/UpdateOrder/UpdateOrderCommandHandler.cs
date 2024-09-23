using SalesCore.Application.Abstractions.Data;
using SalesCore.Application.Abstractions.Messaging;
using SalesCore.Application.Orders.CreateOrder;
using SalesCore.Domain.Abstractions;
using SalesCore.Domain.Orders;

namespace SalesCore.Application.Orders.UpdateOrder;

internal sealed class UpdateOrderCommandHandler(IOrderRepository orderRepository, IDbContext dbContext) : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
{
    public async Task<Result<UpdateOrderResult>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Order.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure<UpdateOrderResult>(OrderErrors.NotFound);

        var requestItems = request.Order.OrderItems.ToDictionary(oi => oi.ProductId);

        foreach (var item in order.OrderItems)
        {
            if (!requestItems.ContainsKey(item.ProductId))
                order.CancelItem(item.ProductId);
        }

        foreach (var item in requestItems.Values)
        {
            order.AddItem(item.ProductId, item.Quantity, item.Price);
        }
        orderRepository.Update(order);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateOrderResult(order.Id));
    }
}
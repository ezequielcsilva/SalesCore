using SalesCore.Application.Abstractions.Data;
using SalesCore.Application.Abstractions.Messaging;
using SalesCore.Domain.Abstractions;
using SalesCore.Domain.Orders;

namespace SalesCore.Application.Orders.DeleteOrder;

internal sealed class DeleteOrderCommandHandler(IOrderRepository orderRepository, IDbContext dbContext) : ICommandHandler<DeleteOrderCommand>
{
    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);
        orderRepository.Delete(order);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
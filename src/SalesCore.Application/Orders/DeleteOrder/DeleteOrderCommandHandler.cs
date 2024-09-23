using SalesCore.Application.Abstractions.Messaging;
using SalesCore.Domain.Abstractions;
using SalesCore.Domain.Orders;

namespace SalesCore.Application.Orders.DeleteOrder;

internal sealed class DeleteOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<DeleteOrderCommand>
{
    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        orderRepository.Delete(order);

        // Todo: Chamada para salvar as alterações (SaveChanges)

        return Result.Success();
    }
}
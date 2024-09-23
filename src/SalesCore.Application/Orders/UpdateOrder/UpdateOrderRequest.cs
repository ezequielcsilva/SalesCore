namespace SalesCore.Application.Orders.UpdateOrder;

public sealed record UpdateOrderRequest(
    Guid OrderId,
    List<UpdateOrderItemRequest> OrderItems);

public sealed record UpdateOrderItemRequest(Guid ProductId, int Quantity, decimal Price);
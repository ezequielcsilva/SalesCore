using SalesCore.Domain.Orders;

namespace SalesCore.Application.Orders.GetOrderById;

public sealed record GetOrderByIdResult(
    Guid OrderId,
    DateTime Date,
    Guid CustomerId,
    Guid BranchId,
    decimal TotalAmount,
    decimal Discount,
    decimal TotalCancelledAmount,
    IEnumerable<GetOrderByIdItemsResult> Items,
    OrderStatus Status);

public sealed record GetOrderByIdItemsResult(
    Guid OrderItemId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalItemAmount,
    bool IsCancelled);
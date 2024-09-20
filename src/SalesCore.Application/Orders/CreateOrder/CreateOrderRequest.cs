using SalesCore.Domain.Orders;

namespace SalesCore.Application.Orders.CreateOrder;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    Guid BranchId,
    decimal Amount,
    List<CreateOrderItemRequest> OrderItems,
    string Voucher,
    bool HasVoucher,
    decimal Discount);

public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity, decimal Price);
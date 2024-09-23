using SalesCore.Domain.Abstractions;

namespace SalesCore.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error NotFound = new(
        "Order.NotFound",
        "Order not found."
    );

    public static readonly Error TotalAmountMismatch = new(
        "Order.TotalAmountMismatch",
        "The order total amount is different from the total amount of individual items."
    );

    public static readonly Error SentAmountMismatch = new(
        "Order.SentAmountMismatch",
        "The amount sent is different from the order amount."
    );
}
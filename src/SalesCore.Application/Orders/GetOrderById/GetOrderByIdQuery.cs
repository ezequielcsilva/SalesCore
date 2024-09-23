using SalesCore.Application.Abstractions.Messaging;

namespace SalesCore.Application.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<GetOrderByIdResult>;
using SalesCore.Application.Abstractions.Messaging;

namespace SalesCore.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(CreateOrderRequest Order) : ICommand<CreateOrderResult>;
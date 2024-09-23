using SalesCore.Application.Abstractions.Messaging;
using SalesCore.Application.Orders.CreateOrder;

namespace SalesCore.Application.Orders.UpdateOrder;

public sealed record UpdateOrderCommand(UpdateOrderRequest Order) : ICommand<UpdateOrderResult>;
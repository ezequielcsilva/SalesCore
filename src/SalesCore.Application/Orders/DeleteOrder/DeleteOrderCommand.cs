using SalesCore.Application.Abstractions.Messaging;

namespace SalesCore.Application.Orders.DeleteOrder;

public sealed record DeleteOrderCommand(Guid OrderId) : ICommand;
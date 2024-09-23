using FluentValidation;

namespace SalesCore.Application.Orders.UpdateOrder;

internal sealed class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidator()
    {
        RuleFor(c => c.Order.OrderItems.Count)
            .GreaterThan(0)
            .WithMessage("The order needs to have at least 1 item");
    }
}
using FluentValidation;

namespace SalesCore.Application.Orders.UpdateOrder;

internal sealed class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidator()
    {
        RuleFor(c => c.Order.OrderItems.Count)
            .GreaterThan(0)
            .WithMessage("The order needs to have at least 1 item");

        RuleForEach(c => c.Order.OrderItems).ChildRules(items =>
        {
            items.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("The quantity of each item must be greater than zero");

            items.RuleFor(i => i.Price)
                .GreaterThan(0)
                .WithMessage("The price of each item must be greater than zero");
        });
    }
}
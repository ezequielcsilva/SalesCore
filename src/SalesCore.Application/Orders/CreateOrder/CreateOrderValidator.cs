using FluentValidation;

namespace SalesCore.Application.Orders.CreateOrder;

internal sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(c => c.Order.CustomerId)
            .NotEqual(Guid.Empty)
            .WithMessage("Invalid customer id");

        RuleFor(c => c.Order.BranchId)
            .NotEqual(Guid.Empty)
            .WithMessage("Invalid branch id");

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

        RuleFor(c => c.Order.Amount)
            .Equal(c => c.Order.OrderItems.Sum(i => i.Quantity * i.Price))
            .WithMessage("The total amount must be equal to the sum of quantity * price for each item.");
    }
}
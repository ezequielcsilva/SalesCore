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
    }
}
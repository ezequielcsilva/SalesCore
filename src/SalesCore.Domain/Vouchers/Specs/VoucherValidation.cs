using SalesCore.Domain.Abstractions;

namespace SalesCore.Domain.Vouchers.Specs;

public static class VoucherValidation
{
    public static Error[] Validate(Voucher voucher)
    {
        var errors = new List<Error>();

        if (!new VoucherDateSpecification().IsSatisfiedBy(voucher))
        {
            errors.Add(VoucherErrors.Expired);
        }

        if (!new VoucherQuantitySpecification().IsSatisfiedBy(voucher))
        {
            errors.Add(VoucherErrors.QuantityExceeded);
        }

        if (!new VoucherActiveSpecification().IsSatisfiedBy(voucher))
        {
            errors.Add(VoucherErrors.NotActive);
        }

        return errors.ToArray();
    }
}
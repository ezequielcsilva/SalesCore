using SalesCore.Domain.Abstractions;

namespace SalesCore.Domain.Vouchers;

public static class VoucherErrors
{
    public static readonly Error VoucherNotFound = new(
        "Voucher.VoucherNotFound",
        "Voucher not found"
    );

    public static readonly Error Expired = new(
        "Voucher.Expired",
        "This voucher is expired."
    );

    public static readonly Error QuantityExceeded = new(
        "Voucher.QuantityExceeded",
        "This voucher has already been used."
    );

    public static readonly Error NotActive = new(
        "Voucher.NotActive",
        "This voucher is no longer active."
    );
}
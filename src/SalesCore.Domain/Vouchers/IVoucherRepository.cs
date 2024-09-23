namespace SalesCore.Domain.Vouchers;

public interface IVoucherRepository
{
    Task<Voucher?> GetVoucherByCodeAsync(string code, CancellationToken cancellationToken = default);

    void Update(Voucher voucher);
}
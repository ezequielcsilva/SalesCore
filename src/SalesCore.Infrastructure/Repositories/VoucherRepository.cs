using Microsoft.EntityFrameworkCore;
using SalesCore.Domain.Vouchers;

namespace SalesCore.Infrastructure.Repositories;

internal sealed class VoucherRepository(ApplicationDbContext dbContext) : IVoucherRepository
{
    public async Task<Voucher?> GetVoucherByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Set<Voucher>()
            .FirstOrDefaultAsync(o => o.Code == code, cancellationToken);
    }

    public void Update(Voucher voucher)
    {
        dbContext.Entry(voucher).State = EntityState.Modified;
        dbContext.Set<Voucher>().Update(voucher);
    }
}
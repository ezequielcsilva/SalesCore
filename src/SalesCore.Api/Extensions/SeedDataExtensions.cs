using SalesCore.Domain.Vouchers;
using SalesCore.Infrastructure;

namespace SalesCore.Api.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Set<Voucher>().Any())
            return;

        var localExpirationDate = DateTime.Now.AddYears(5).ToUniversalTime();
        context.Set<Voucher>().Add(Voucher.Create("30-OFF", 30, 0, 5000, VoucherDiscountType.Percentage, localExpirationDate));
        context.Set<Voucher>().Add(Voucher.Create("50-OFF", 0, 50, 5000, VoucherDiscountType.Value, localExpirationDate));

        context.SaveChanges();
    }
}
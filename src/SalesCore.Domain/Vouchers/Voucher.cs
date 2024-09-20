using SalesCore.Domain.Abstractions;
using SalesCore.Domain.Vouchers.Specs;

namespace SalesCore.Domain.Vouchers;

public sealed class Voucher : Entity, IAggregateRoot
{
    public string Code { get; private set; } = default!;
    public decimal? Percentage { get; private set; }
    public decimal? Discount { get; private set; }
    public int Quantity { get; private set; }
    public VoucherDiscountType DiscountType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public bool Active { get; private set; }
    public bool Used { get; private set; }

    private Voucher() {}

    private Voucher(Guid id, string code, decimal? percentage, decimal? discount, int quantity, VoucherDiscountType discountType, DateTime expirationDate) : base(id)
    {
        Code = code;
        Percentage = percentage;
        Discount = discount;
        Quantity = quantity;
        DiscountType = discountType;
        ExpirationDate = expirationDate;

        CreatedAt = DateTime.Now;
        Active = true;
        Used = false;
    }

    public static Voucher Create(string code, decimal? percentage, decimal? discount, int quantity,
        VoucherDiscountType discountType, DateTime expirationDate)
    {
        var voucher = new Voucher(Guid.NewGuid(), code, percentage, discount, quantity, discountType, expirationDate);

        return voucher;
    }

    public bool CanUse()
    {
        return new VoucherActiveSpecification()
            .And(new VoucherDateSpecification())
            .And(new VoucherQuantitySpecification())
            .IsSatisfiedBy(this);
    }

    public void SetAsUsed()
    {
        Active = false;
        Used = true;
        Quantity = 0;
        UsedAt = DateTime.Now;
    }

    public void GetOne()
    {
        Quantity -= 1;
        if (Quantity >= 1) return;

        SetAsUsed();
    }
}
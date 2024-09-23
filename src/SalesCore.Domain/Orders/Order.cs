using SalesCore.Domain.Abstractions;
using SalesCore.Domain.Vouchers;

namespace SalesCore.Domain.Orders;

public sealed class Order : Entity, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = [];

    private Order()
    { }

    private Order(Guid id, Guid customerId, Guid branchId, DateTime dateAdded, OrderStatus orderStatus,
                  bool hasVoucher = false, decimal discount = 0, Guid? voucherId = null)
        : base(id)
    {
        CustomerId = customerId;
        BranchId = branchId;
        DateAdded = dateAdded;
        OrderStatus = orderStatus;
        HasVoucher = hasVoucher;
        Discount = discount;
        VoucherId = voucherId;
    }

    public long Code { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid? VoucherId { get; private set; }
    public bool HasVoucher { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal CancelledItemsAmount { get; private set; }
    public DateTime DateAdded { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public Voucher? Voucher { get; private set; }

    public static Order Create(Guid customerId, Guid branchId, DateTime dateAdded,
                               bool hasVoucher = false, decimal discount = 0, Guid? voucherId = null)
    {
        return new Order(Guid.NewGuid(), customerId, branchId, dateAdded,
                         OrderStatus.Pending, hasVoucher, discount, voucherId);
    }

    public void AddItem(Guid productId, int quantity, decimal price)
    {
        ValidateItemInputs(quantity, price);

        var existingItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.Update(quantity, price);
        }
        else
        {
            _orderItems.Add(OrderItem.Create(productId, quantity, price));
        }

        RecalculateOrderAmounts();
    }

    public void CancelItem(Guid productId)
    {
        _orderItems.FirstOrDefault(x => x.ProductId == productId)?.Cancel();
        RecalculateOrderAmounts();
    }

    public void AssociateVoucher(Voucher voucher)
    {
        HasVoucher = true;
        VoucherId = voucher.Id;
        Voucher = voucher;
        RecalculateOrderAmounts();
    }

    private static void ValidateItemInputs(int quantity, decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));
    }

    internal void RecalculateOrderAmounts()
    {
        TotalAmount = _orderItems
            .Where(item => !item.Cancelled)
            .Sum(item => item.GetAmount());

        CancelledItemsAmount = _orderItems
            .Where(item => item.Cancelled)
            .Sum(item => item.GetAmount());

        ApplyVoucherDiscount();
    }

    private void ApplyVoucherDiscount()
    {
        if (!HasVoucher) return;

        decimal discount = 0;
        var total = TotalAmount;

        if (Voucher is { DiscountType: VoucherDiscountType.Percentage, Percentage: not null })
        {
            discount = (total * Voucher.Percentage.Value) / 100;
        }
        else if (Voucher?.Discount is not null)
        {
            discount = Voucher.Discount.Value;
        }

        TotalAmount = Math.Max(total - discount, 0);
        Discount = discount;
    }
}
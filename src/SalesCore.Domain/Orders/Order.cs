using SalesCore.Domain.Abstractions;

namespace SalesCore.Domain.Orders;

public sealed class Order : Entity, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = [];

    private Order()
    { }

    private Order(Guid id, Guid customerId, Guid branchId, DateTime utcNow, OrderStatus orderStatus, decimal discount = 0)
        : base(id)
    {
        CustomerId = customerId;
        BranchId = branchId;
        Discount = discount;
        DateAdded = utcNow;
        OrderStatus = orderStatus;
    }

    public long Code { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid BranchId { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal CancelledItemsAmount { get; private set; }
    public DateTime DateAdded { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Order Create(Guid customerId, Guid branchId, DateTime utcNow, decimal discount = 0)
    {
        var order = new Order(Guid.NewGuid(), customerId, branchId, utcNow, OrderStatus.Pending, discount);
        return order;
    }

    private void CalculateOrderAmount()
    {
        var activeItemsAmount = _orderItems
            .Where(item => !item.Cancelled)
            .Sum(item => item.GetAmount());

        CancelledItemsAmount = _orderItems
            .Where(item => item.Cancelled)
            .Sum(item => item.GetAmount());

        TotalAmount = activeItemsAmount - Discount;

        TotalAmount = Math.Max(0, TotalAmount);
    }

    public void AddItem(Guid productId, int quantity, decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        var existingItem = OrderItems.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.Update(quantity, price);
        }
        else
        {
            _orderItems.Add(OrderItem.Create(productId, quantity, price));
        }

        CalculateOrderAmount();
    }

    public void CancelItem(Guid productId)
    {
        _orderItems.FirstOrDefault(x => x.ProductId == productId)?.Cancel();
        CalculateOrderAmount();
    }
}
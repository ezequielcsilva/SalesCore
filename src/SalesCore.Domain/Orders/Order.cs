using SalesCore.Domain.Abstractions;

namespace SalesCore.Domain.Orders;

public sealed class Order : Entity, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems;

    private Order() : base(Guid.NewGuid())
    {
        _orderItems = new List<OrderItem>();
    }

    private Order(Guid id, Guid customerId, Guid branchId, DateTime utcNow, IEnumerable<OrderItem> orderItems, OrderStatus orderStatus, decimal discount = 0)
        : base(id)
    {
        CustomerId = customerId;
        BranchId = branchId;
        Discount = discount;
        DateAdded = utcNow;
        OrderStatus = orderStatus;
        _orderItems = orderItems.ToList();
        CalculateOrderAmount();
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

    public static Order Create(Guid customerId, Guid branchId, DateTime utcNow, IEnumerable<OrderItem> orderItems, decimal discount = 0)
    {
        var order = new Order(Guid.NewGuid(), customerId, branchId, utcNow, orderItems, OrderStatus.Created, discount);
        return order;
    }

    private void CalculateOrderAmount()
    {
        var activeItemsAmount = _orderItems
            .Where(item => !item.IsCancelled)
            .Sum(item => item.GetAmount());

        CancelledItemsAmount = _orderItems
            .Where(item => item.IsCancelled)
            .Sum(item => item.GetAmount());

        TotalAmount = activeItemsAmount - Discount;

        TotalAmount = Math.Max(0, TotalAmount);
    }
}
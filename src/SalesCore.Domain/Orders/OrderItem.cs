using SalesCore.Domain.Abstractions;

namespace SalesCore.Domain.Orders;

public sealed class OrderItem : Entity
{
    private OrderItem()
    { }

    private OrderItem(Guid id, Guid productId, int quantity, decimal price) : base(id)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        ProductId = productId;
        Quantity = quantity;
        Price = price;
        Cancelled = false;
    }

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public bool Cancelled { get; private set; }

    public static OrderItem Create(Guid productId, int quantity, decimal price)
        => new(Guid.NewGuid(), productId, quantity, price);

    public decimal GetAmount()
    {
        return Quantity * Price;
    }

    public void Cancel() => Cancelled = true;

    public void Update(int quantity, decimal price)
    {
        Quantity = quantity;
        Price = price;
        Cancelled = false;
    }
}
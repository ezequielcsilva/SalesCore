namespace SalesCore.Domain.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    void Add(Order order);

    void Update(Order order);

    void Delete(Order order);
}
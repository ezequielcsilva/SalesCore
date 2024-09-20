namespace SalesCore.Domain.Orders;

public interface IOrderRepository
{
    void Add(Order order);
}
using Microsoft.EntityFrameworkCore;
using SalesCore.Domain.Orders;

namespace SalesCore.Infrastructure.Repositories;

internal sealed class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Set<Order>()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public void Add(Order order)
    {
        dbContext.Set<Order>().Add(order);
    }

    public void Update(Order order)
    {
        dbContext.Entry(order).State = EntityState.Modified;
        dbContext.Set<Order>().Update(order);
    }

    public void Delete(Order order)
    {
        dbContext.Set<Order>().Remove(order);
    }
}
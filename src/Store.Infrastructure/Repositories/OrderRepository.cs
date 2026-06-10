using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Domain.Interfaces;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class OrderRepository : EfRepository<Order>, IOrderRepository
{
    private readonly StoreDbContext _dbContext;

    public OrderRepository(StoreDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<Order> Orders, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(order => order.Product)
            .AsNoTracking()
            .AsQueryable();

        if (orderDateFrom.HasValue)
        {
            query = query.Where(order => order.OrderDate >= orderDateFrom.Value);
        }

        if (orderDateTo.HasValue)
        {
            query = query.Where(order => order.OrderDate <= orderDateTo.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (orders, totalCount);
    }

    public async Task<Order?> GetWithProductAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(order => order.Product)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }
}

using Store.Domain.Entities;

namespace Store.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<(IReadOnlyList<Order> Orders, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        CancellationToken cancellationToken = default);

    Task<Order?> GetWithProductAsync(int id, CancellationToken cancellationToken = default);
}

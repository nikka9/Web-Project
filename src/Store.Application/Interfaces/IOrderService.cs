using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface IOrderService
{
    Task<PagedResponse<OrderReadDto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        CancellationToken cancellationToken = default);

    Task<OrderReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken cancellationToken = default);

    Task<OrderReadDto> UpdateAsync(int id, OrderCreateDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

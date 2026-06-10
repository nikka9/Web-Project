using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(
        decimal? minPrice,
        decimal? maxPrice,
        int? categoryId,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken = default);

    Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken = default);

    Task<ProductDto> UpdateAsync(int id, ProductDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

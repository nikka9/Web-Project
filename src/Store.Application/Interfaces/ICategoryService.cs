using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken cancellationToken = default);

    Task<CategoryDto> UpdateAsync(int id, CategoryDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

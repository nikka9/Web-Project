using Store.Application.DTOs;
using Store.Application.Exceptions;
using Store.Application.Interfaces;
using Store.Domain.Entities;
using Store.Domain.Interfaces;

namespace Store.Application.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;

    public ProductService(
        IRepository<Product> productRepository,
        IRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(
        decimal? minPrice,
        decimal? maxPrice,
        int? categoryId,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken = default)
    {
        if (minPrice is < 0)
        {
            throw new AppValidationException("minPrice cannot be negative.");
        }

        if (maxPrice is < 0)
        {
            throw new AppValidationException("maxPrice cannot be negative.");
        }

        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
        {
            throw new AppValidationException("minPrice cannot be greater than maxPrice.");
        }

        if (categoryId.HasValue && await _categoryRepository.GetByIdAsync(categoryId.Value, cancellationToken) is null)
        {
            throw new AppValidationException($"Category with id {categoryId.Value} does not exist.");
        }

        var products = (await _productRepository.GetAllAsync(cancellationToken)).AsEnumerable();

        if (minPrice.HasValue)
        {
            products = products.Where(product => product.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            products = products.Where(product => product.Price <= maxPrice.Value);
        }

        if (categoryId.HasValue)
        {
            products = products.Where(product => product.CategoryId == categoryId.Value);
        }

        products = ApplySorting(products, sortBy, sortDirection);

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {id} was not found.");

        return MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCategoryExistsAsync(dto.CategoryId, cancellationToken);

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            CategoryId = dto.CategoryId
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {id} was not found.");

        await EnsureCategoryExistsAsync(dto.CategoryId, cancellationToken);

        product.Name = dto.Name.Trim();
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {id} was not found.");

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.GetByIdAsync(categoryId, cancellationToken) is null)
        {
            throw new AppValidationException($"Category with id {categoryId} does not exist.");
        }
    }

    private static IEnumerable<Product> ApplySorting(
        IEnumerable<Product> products,
        string? sortBy,
        string? sortDirection)
    {
        var normalizedSortBy = sortBy?.Trim();
        var normalizedDirection = string.IsNullOrWhiteSpace(sortDirection)
            ? "asc"
            : sortDirection.Trim().ToLowerInvariant();

        if (normalizedDirection is not "asc" and not "desc")
        {
            throw new AppValidationException("sortDirection must be 'asc' or 'desc'.");
        }

        if (string.IsNullOrWhiteSpace(normalizedSortBy))
        {
            return products.OrderBy(product => product.Id);
        }

        return normalizedSortBy.ToLowerInvariant() switch
        {
            "price" => normalizedDirection == "asc"
                ? products.OrderBy(product => product.Price)
                : products.OrderByDescending(product => product.Price),
            "name" => normalizedDirection == "asc"
                ? products.OrderBy(product => product.Name)
                : products.OrderByDescending(product => product.Name),
            _ => throw new AppValidationException("sortBy must be 'Price' or 'Name'.")
        };
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId
        };
    }
}

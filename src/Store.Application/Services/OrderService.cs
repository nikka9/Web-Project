using Store.Application.DTOs;
using Store.Application.Exceptions;
using Store.Application.Interfaces;
using Store.Domain.Entities;
using Store.Domain.Interfaces;

namespace Store.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<Product> _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IRepository<Product> productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<PagedResponse<OrderReadDto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
        {
            throw new AppValidationException("pageNumber must be greater than or equal to 1.");
        }

        if (pageSize < 1)
        {
            throw new AppValidationException("pageSize must be greater than or equal to 1.");
        }

        if (pageSize > 100)
        {
            throw new AppValidationException("pageSize cannot be greater than 100.");
        }

        if (orderDateFrom.HasValue && orderDateTo.HasValue && orderDateFrom > orderDateTo)
        {
            throw new AppValidationException("orderDateFrom cannot be greater than orderDateTo.");
        }

        var (orders, totalCount) = await _orderRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            orderDateFrom,
            orderDateTo,
            cancellationToken);

        return new PagedResponse<OrderReadDto>
        {
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            CurrentPage = pageNumber,
            Data = orders.Select(MapToDto).ToList()
        };
    }

    public async Task<OrderReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithProductAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Order with id {id} was not found.");

        return MapToDto(order);
    }

    public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken cancellationToken = default)
    {
        EnsureValidQuantity(dto.Quantity);

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken)
            ?? throw new AppValidationException($"Product with id {dto.ProductId} does not exist.");

        var order = new Order
        {
            CustomerName = dto.CustomerName.Trim(),
            OrderDate = DateTime.UtcNow,
            ProductId = product.Id,
            Product = product,
            Quantity = dto.Quantity,
            ProductPriceAtOrderTime = product.Price
        };

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderReadDto> UpdateAsync(int id, OrderCreateDto dto, CancellationToken cancellationToken = default)
    {
        EnsureValidQuantity(dto.Quantity);

        var order = await _orderRepository.GetWithProductAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Order with id {id} was not found.");

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken)
            ?? throw new AppValidationException($"Product with id {dto.ProductId} does not exist.");

        order.CustomerName = dto.CustomerName.Trim();
        order.ProductId = product.Id;
        order.Product = product;
        order.Quantity = dto.Quantity;
        order.ProductPriceAtOrderTime = product.Price;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Order with id {id} was not found.");

        _orderRepository.Delete(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureValidQuantity(int quantity)
    {
        if (quantity < 1)
        {
            throw new AppValidationException("Quantity must be greater than or equal to 1.");
        }
    }

    private static OrderReadDto MapToDto(Order order)
    {
        var totalPrice = order.Quantity * order.ProductPriceAtOrderTime;

        return new OrderReadDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            ProductId = order.ProductId,
            ProductName = order.Product?.Name ?? string.Empty,
            Quantity = order.Quantity,
            ProductPriceAtOrderTime = order.ProductPriceAtOrderTime,
            TotalPrice = totalPrice,
            IsExpensive = totalPrice > 100
        };
    }
}

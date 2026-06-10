using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs;
using Store.Application.Interfaces;
using Store.Domain.Constants;

namespace Store.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? categoryId,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(
            minPrice,
            maxPrice,
            categoryId,
            sortBy,
            sortDirection,
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        ProductDto dto,
        CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(
        int id,
        ProductDto dto,
        CancellationToken cancellationToken)
    {
        var product = await _productService.UpdateAsync(id, dto, cancellationToken);
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

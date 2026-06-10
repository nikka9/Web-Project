using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs;
using Store.Application.Interfaces;

namespace Store.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderReadDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? orderDateFrom = null,
        [FromQuery] DateTime? orderDateTo = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _orderService.GetPagedAsync(
            pageNumber,
            pageSize,
            orderDateFrom,
            orderDateTo,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderReadDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        return Ok(order);
    }

    [Authorize(Roles = "admin,user")]
    [HttpPost]
    public async Task<ActionResult<OrderReadDto>> Create(
        OrderCreateDto dto,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderReadDto>> Update(
        int id,
        OrderCreateDto dto,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateAsync(id, dto, cancellationToken);
        return Ok(order);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _orderService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

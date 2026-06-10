using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs;

public class OrderCreateDto
{
    [Required]
    [MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 1.")]
    public int Quantity { get; set; }
}

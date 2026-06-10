using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0.")]
    public int CategoryId { get; set; }
}

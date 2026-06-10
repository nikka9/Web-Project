namespace Store.Application.DTOs;

public class OrderReadDto
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal ProductPriceAtOrderTime { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsExpensive { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs;

public class LoginDto
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}

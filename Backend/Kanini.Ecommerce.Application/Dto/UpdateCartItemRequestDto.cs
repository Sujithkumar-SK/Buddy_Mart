using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class UpdateCartItemRequestDto
{
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}
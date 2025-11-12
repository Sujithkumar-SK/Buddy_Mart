using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class ProductUpdateRequestDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string Name { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Discount price cannot be negative")]
    public decimal? DiscountPrice { get; set; }

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Min stock level cannot be negative")]
    public int? MinStockLevel { get; set; }

    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string? Brand { get; set; }

    [StringLength(50, ErrorMessage = "Weight cannot exceed 50 characters")]
    public string? Weight { get; set; }

    [StringLength(100, ErrorMessage = "Dimensions cannot exceed 100 characters")]
    public string? Dimensions { get; set; }

    [Required(ErrorMessage = "Category ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid category ID")]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}
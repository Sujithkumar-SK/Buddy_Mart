namespace Kanini.Ecommerce.Application.DTOs;

public class ProductListDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? Brand { get; set; }
    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = null!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string? PrimaryImagePath { get; set; }
    public List<string> ImagePaths { get; set; } = new List<string>();
}